import asyncio
import aiohttp
import logging
import argparse
import os

from monte_client import MonteClient, AuthClient
from config import create_config, validate_config
from logger import configure_logger


CONFIG_FILE = './config.yaml'
ENVIRONMENT = 'development' # production, development

async def main():
    configure_logger(CONFIG_FILE)
    logger = logging.getLogger()

    config = create_config(CONFIG_FILE, ENVIRONMENT)
    validate_config(config)

    async with aiohttp.ClientSession(config.auth_url) as auth_session:
        auth_client = AuthClient(auth_session, config)
        async with aiohttp.ClientSession(config.api_url) as api_session:
            api_client = MonteClient(api_session, auth_client, config)
            await run(api_client, config)

async def run(api: MonteClient, config):
    logger = logging.getLogger()
    while True:
        try:
            await api.push_report()
            await asyncio.sleep(config.reporting_period)
        except Exception as e:
            logger.error(f'Exception occurred.', exc_info=e)
    

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Pass arguments to the monte agent')
    parser.add_argument('--config', type=str, help='Select environment from config.yaml - production or development', required=False)
    ENVIRONMENT = parser.parse_args().config or ENVIRONMENT
    print(os.getcwd())

    try:
        asyncio.run(main())
    finally:
        logging.getLogger().info("Exiting program.\n")