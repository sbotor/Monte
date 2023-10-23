import aiohttp
from datetime import datetime, timedelta
import utils

DEFAULT_TOKEN_ENDPOINT = '/connect/token'

_SSL = False

_CLIENT_ID = 'monte_agent'
_CLIENT_SECRET = '2VoONsTTvGCryUrTxMY0'

class AuthClient:
    def __init__(self, session: aiohttp.ClientSession):
        self._session = session
        self._token = None
        self._expires = datetime.min

        self.token_endpoint = DEFAULT_TOKEN_ENDPOINT

    @property
    def token(self):
        return self._token
    
    @property
    def is_expired(self):
        return not self.token or datetime.utcnow() > self._expires
    
    async def authenticate(self):
        body = {
            'client_id': _CLIENT_ID,
            'client_secret': _CLIENT_SECRET,
            'grant_type': 'client_credentials',
            'scope': 'monte_agent_api'
        }

        async with self._session.post(self.token_endpoint, data=body, ssl=_SSL) as resp:
            if resp.status != 200:
                return False
            
            resp_json = await resp.json()
            self._token = resp_json['access_token']
            self._expires = datetime.utcnow() + timedelta(seconds=resp_json['expires_in'])

            print(self._token)
            return True


class MonteClient:
    def __init__(self, session: aiohttp.ClientSession, authClient: AuthClient):
        self._session = session
        self._auth = authClient

        self._origin = utils.extract_hostname()

    async def _execute(self, method: str, path = '', **kwargs):
        if not await self._ensure_auth():
            return None
        
        headers = {
            'Authorization': 'Bearer ' + self._token,
            'Origin': self._origin
        }
        result = await self._session.request(method, f'/api/{path}', headers=headers, ssl=_SSL, **kwargs)

        if result.status >= 200 and result.status < 300:
            return await result.text()
        
        return None

    async def _ensure_auth(self):
        if not self._auth.is_expired:
            return True
        
        success = await self._auth.authenticate()
        if not success:
            print('Could not authenticate.')
        
        return success
            