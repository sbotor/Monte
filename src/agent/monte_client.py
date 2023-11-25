import aiohttp
from datetime import datetime, timedelta
import utils
import config
import logging
import monitoring

class AuthClient:
    def __init__(self, session: aiohttp.ClientSession, config: config.Config):
        self._session = session
        self._token = None
        self._expires = datetime.min
        self._config = config
        self._logger = logging.getLogger(__name__)

    @property
    def token(self):
        return self._token
    
    @property
    def is_expired(self):
        return not self.token or datetime.utcnow() > self._expires
    
    async def authenticate(self):
        body = {
            'client_id': self._config.client_id,
            'client_secret': self._config.client_secret,
            'grant_type': 'client_credentials',
            'scope': 'openid roles monte_agent_api'
        }

        self._logger.info('Authenticating.')
        async with await self._session.post('/connect/token', data=body, ssl=self._config.enable_ssl) as resp:
            if resp.status != 200:
                self._logger.warning('Could not authenticate. Status: %s.', resp.status)
                return False
            
            resp_json = await resp.json()
            self._token = resp_json['access_token']
            self._expires = datetime.utcnow() + timedelta(seconds=resp_json['expires_in'])

            return True

class MonteClient:
    def __init__(self, session: aiohttp.ClientSession, authClient: AuthClient, config: config.Config):
        self._session = session
        self._auth = authClient
        self._config = config
        self._origin = utils.extract_hostname()
        self._id = config.id
        self._initialized = False

        self._logger = logging.getLogger(__name__)

    async def _default_resp_handler(self, resp: aiohttp.ClientResponse):
        resp_text = await resp.text()
        if not (resp.status >= 200 and resp.status < 300):
            self._logger.warn('Error executing %s "%s": %s. %s', resp.method, resp.url, resp.status, resp_text)
            return None

        return resp_text
    
    async def _execute_core(self, method: str, path='', resp_handler=None, **kwargs):
        resp_handler = resp_handler or self._default_resp_handler
        
        async with await self._session.request(method, f'/api/{path}', ssl=self._config.enable_ssl, **kwargs) as resp:
            return await resp_handler(resp)

    async def _execute(self, method: str, path='', **kwargs):
        if not await self._ensure_auth():
            return None
        
        if not await self._ensure_init():
            return None
        
        headers = {
            'Authorization': 'Bearer ' + self._auth._token,
            'Origin': self._origin,
            'Agent-Id': self._id
        }
        
        return await self._execute_core(method, path, None, headers=headers, **kwargs)
    
    async def _initialize(self):
        monitoring.initialize_monitoring()
        
        if not await self._ensure_auth():
            return ''
        
        headers = {
            'Authorization': 'Bearer ' + self._auth._token,
            'Origin': self._origin
        }

        if self._id:
            headers['Agent-Id'] = self._id

        body = monitoring.get_system_info()

        return await self._execute_core('PUT', 'agentMetrics/init', json=body, headers=headers)
    
    async def _ensure_init(self):
        if self._initialized:
            return True
        
        self._id = await self._initialize()
        if not self._id:
            print('Could not initialize agent ID.')
            return False
        
        self._initialized = True
        utils.store_agent_id(self._id)
            
        return True

    async def _ensure_auth(self):
        if not self._auth.is_expired:
            return True
        
        success = await self._auth.authenticate()
        if not success:
            print('Could not authenticate.')
        
        return success
    
    async def push_report(self):
        await self._ensure_init()

        body = monitoring.get_system_resources(self._config.reporting_period)
        await self._execute('POST', 'agentMetrics', json=body)
            