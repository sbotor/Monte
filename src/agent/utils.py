import socket
import yaml

def extract_hostname():
    return socket.gethostname();

def get_config(config_file):
    with open(config_file, 'r') as stream:
        try:
            config = yaml.load(stream)
            return config
        except yaml.YAMLError as exc:
            print(exc)
            return None

def get_defaults(config):
    return config['defaults']

def get_environment(enviroment, config):
    return config['environments'][enviroment]

def create_config(config_file, environment):
    config = get_config(config_file)
    defaults = get_defaults(config)
    envs = get_environment(environment, config)

    for key in envs:
        defaults[key] = envs[key]
    
    return defaults

class Config:
    def __init__(self, config_file, environment):
        self._config = create_config(config_file, environment)

    @property
    def auth_url(self) -> str:
        return self._config['auth_url']
    
    @property
    def api_url(self) -> str:
        return self._config['api_url']
    
    @property
    def reporting_period(self) -> int:
        return self._config['reporting_period']
    
    @property
    def name(self) -> str:
        return self._config['name'] or extract_hostname()

    @property
    def client_id(self) -> str:
        return self._config['client_id']
    
    @property
    def client_secret(self) -> str:
        return self._config['client_secret']
    
    @property
    def disable_ssl(self) -> bool:
        return self._config['disable_ssl']
    

    

    