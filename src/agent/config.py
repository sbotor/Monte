import yaml
import logging

from utils import extract_hostname, read_agent_id

def _get_config(config_file):
    logging.info('Reading config from %s.', config_file)
    with open(config_file, 'r') as stream:
        config = yaml.safe_load(stream)
        return config

def _get_defaults(config):
    return config['defaults']

def _get_environment(enviroment, config):
    return config['environments'][enviroment]

def create_config(config_file: str, environment: str):
    root_config = _get_config(config_file)
    config = _get_defaults(root_config)
    env_overrides = _get_environment(environment, root_config)

    for key in env_overrides:
        config[key] = env_overrides[key]
    
    return Config(config)

def validate_config(config: 'Config'):
    # TODO: validate config
    return

class Config:
    def __init__(self, config_dict: 'dict[str]'):
        self._config = config_dict
        self._id = None

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
            id = read_agent_id()
            self._id = '' if id is None else id
            
        return self._id