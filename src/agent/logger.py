import logging
import logging.handlers
import yaml
from pathlib import Path

LOGGING_FORMAT = '%(asctime)s %(levelname)s %(message)s'

VALID_LEVELS = ['DEBUG', 'INFO', 'WARING', 'ERROR', 'CRITICAL']

def _read_config(config_file):
     with open(config_file, 'r') as stream:
        config = yaml.safe_load(stream)
        return config['logger']

     
def configure_logger(config_file: str):
    config = _read_config(config_file)
    formatter = logging.Formatter(LOGGING_FORMAT)
    rootLogger = logging.getLogger()

    logLevel = config['log_level'].upper()

    if(logLevel not in VALID_LEVELS):
        return

    rootLogger.setLevel(logLevel)

    path = config['logs_path']
    Path(path).mkdir(parents=True, exist_ok=True)

    fileHandler = logging.handlers.TimedRotatingFileHandler(
        filename= f"{path}/log",
        backupCount=30,
        utc=True,
        when='S' 
          )
    fileHandler.setFormatter(formatter)
    fileHandler.setLevel(logLevel)
    rootLogger.addHandler(fileHandler)
    
    consoleHandler = logging.StreamHandler()
    consoleHandler.setFormatter(formatter)
    consoleHandler.setLevel(logLevel)
    rootLogger.addHandler(consoleHandler)   

