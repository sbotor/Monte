import socket

_AGENT_ID_FILENAME = 'agentid'

def extract_hostname():
    return socket.gethostname()

def store_agent_id(value: str):
    pass

def read_agent_id() -> str:
    return ''