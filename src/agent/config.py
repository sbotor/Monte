from pathlib import Path, PosixPath, WindowsPath
from sys import platform
import yaml
import logging

from utils import extract_hostname, read_agent_id

def _get_config(config_file):
    logging.info('Reading config from %s.', Path(config_file).absolute())
    with open(config_file, 'r') as stream:
        config = yaml.safe_load(stream)
        return config

def _get_defaults(config):
    return config['defaults']

def _get_environment(enviroment, config):
    return config['environments'][enviroment]

def _resolve_path(str_path: str):
    if platform == "linux" or platform == "linux2":
        path = PosixPath(str_path).expanduser()
    elif platform == "win32":
        path = WindowsPath("C:\\ProgramData\\Monte")
    else:
        raise OSError(f"Unsupported platform: {platform}")
    
    path.mkdir(parents=True, exist_ok=True)
    return path.absolute()

def create_config(config_file: str, environment: str) -> 'Config':
    root_config = _get_config(config_file)
    config = _get_defaults(root_config)
    env_overrides = _get_environment(environment, root_config)

    for key in env_overrides:
        config[key] = env_overrides[key]
    
    return Config(config, root_config)

class Config:
    def __init__(self, config_dict: 'dict[str]', raw_config: 'dict[str]'):
        self._config = config_dict
        self.raw_config = raw_config
        self._id = None
        self._resources_path = None

    @property
    def auth_url(self) -> str:
        return self._config['auth_url']
    
    @property
    def api_url(self) -> str:
        return self._config['api_url']
    
    @property
    def reporting_period(self) -> int:
        value = self._config.get('reporting_period')
        if value is None:
            value = 5

        return value
    
    @property
    def name(self) -> str:
        return self._config.get('name') or extract_hostname()

    @property
    def client_id(self) -> str:
        return self._config['client_id']
    
    @property
    def client_secret(self) -> str:
        return self._config['client_secret']
    
    @property
    def enable_ssl(self) -> bool:
        value = self._config.get('enable_ssl')
        if value is None:
            value = True

        return value
    
    @property
    def id(self) -> str:
        if self._id is None:
            id = read_agent_id(self.resources_path)
            self._id = '' if id is None else id
            
        return self._id
    
    @property
    def resources_path(self):
        if self._resources_path is not None:
            return self._resources_path

        value = self._config.get('resources_path')
        if value is None:
            raise ValueError('No resources path configured')
        
        self._resources_path = str(_resolve_path(value))
        return self._resources_path