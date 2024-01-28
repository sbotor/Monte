import asyncio
import aiohttp
import logging
import argparse

from monte_client import MonteClient, AuthClient
from config import create_config, validate_config
from logger import configure_logger


CONFIG_FILE = './config.yaml'

class Params:
    def __init__(self, env_name: str):
        self.env_name = env_name
        self.is_stopped = False

def main(params: Params):
    try:
        asyncio.run(_main(params))
    finally:
        logging.getLogger().info("Exiting program.\n")

async def _main(params: Params):
    configure_logger(CONFIG_FILE)

    config = create_config(CONFIG_FILE, params.env_name)
    validate_config(config)

    async with aiohttp.ClientSession(config.auth_url) as auth_session:
        auth_client = AuthClient(auth_session, config)
        async with aiohttp.ClientSession(config.api_url) as api_session:
            api_client = MonteClient(api_session, auth_client, config)
            await _run(api_client, params, config)

async def _run(api: MonteClient, params: Params, config):
    logger = logging.getLogger()
    while not params.is_stopped:
        try:
            await api.push_report()
            await asyncio.sleep(config.reporting_period)
        except Exception as e:
            logger.error(f'Exception occurred.', exc_info=e)
    

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Pass arguments to the monte agent')
    parser.add_argument('--config', type=str, help='Select environment from config.yaml - production or development', required=False)
    env_name = parser.parse_args().config or 'development'
    
    main(Params(env_name))