import asyncio
import aiohttp

from monte_client import MonteClient, AuthClient

SLEEP = 1

AUTH_URL = 'https://localhost:7049'
API_URL = 'https://localhost:7048'

async def main():
    async with aiohttp.ClientSession(AUTH_URL) as auth_session:
        auth_client = AuthClient(auth_session)
        async with aiohttp.ClientSession(API_URL) as api_session:
            api_client = MonteClient(api_session, auth_client)
            while True:
                try:
                    await loop_once(api_client)
                    await asyncio.sleep(SLEEP)
                except:
                    pass

async def loop_once(api: MonteClient):
    await api.get('/api/machines/test')

if __name__ == '__main__':
    asyncio.run(main())