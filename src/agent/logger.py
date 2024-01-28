import logging
import logging.handlers
from pathlib import Path

from config import Config

LOGGING_FORMAT = '%(asctime)s %(levelname)s %(message)s'

VALID_LEVELS = ['DEBUG', 'INFO', 'WARING', 'ERROR', 'CRITICAL']
     
def configure_logger(config: Config):
    formatter = logging.Formatter(LOGGING_FORMAT)
    rootLogger = logging.getLogger()

    logging_config = config.raw_config['logger']
    logLevel = logging_config['log_level'].upper()

    if logLevel not in VALID_LEVELS:
        raise ValueError('Invalid logging level')

    rootLogger.setLevel(logLevel)

    path = config.resources_path + '/logs'
    Path(path).mkdir(parents=True, exist_ok=True)

    fileHandler = logging.handlers.TimedRotatingFileHandler(
        filename= f"{path}/log",
        backupCount=30,
        utc=True,
        when='midnight')
    fileHandler.setFormatter(formatter)
    rootLogger.addHandler(fileHandler)
    
    consoleHandler = logging.StreamHandler()
    consoleHandler.setFormatter(formatter)
    rootLogger.addHandler(consoleHandler)
