import logging
import logging.handlers
import yaml
from pathlib import Path, PosixPath, WindowsPath
from sys import platform

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
    path = resolve_path(path)

    fileHandler = logging.handlers.TimedRotatingFileHandler(
        filename= f"{path}/log",
        backupCount=30,
        utc=True,
        when='midnight'
          )
    fileHandler.setFormatter(formatter)
    rootLogger.addHandler(fileHandler)
    
    consoleHandler = logging.StreamHandler()
    consoleHandler.setFormatter(formatter)
    rootLogger.addHandler(consoleHandler)

def resolve_path(path):
    if platform == "linux" or platform == "linux2":
        path = PosixPath(path).expanduser()
    elif platform == "win32":
        path = WindowsPath(path).expanduser()
    else:
        raise OSError(f"Unsupported platform: {platform}")
    
    path.mkdir(parents=True, exist_ok=True)
    return path.absolute()

