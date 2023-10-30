import asyncio
import aiohttp

from monte_client import MonteClient, AuthClient
from utils import Config

CONFIG_FILE = './config.yml'
ENVIRONMENT = 'production' # production, development, docker

async def main():
    config = Config(CONFIG_FILE, ENVIRONMENT)
    async with aiohttp.ClientSession(config.auth_url) as auth_session:
        auth_client = AuthClient(auth_session, config)
        async with aiohttp.ClientSession(config.api_url) as api_session:
            api_client = MonteClient(api_session, auth_client, config)
            while True:
                try:
                    await loop_once(api_client)
                    await asyncio.sleep(config.reporting_period)
                except:
                    pass

async def loop_once(api: MonteClient):
    await api.get('/api/machines/test')

if __name__ == '__main__':
    asyncio.run(main())