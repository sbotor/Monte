import socket
import os

_AGENT_ID_FILENAME = 'monteid'

def extract_hostname():
    return socket.gethostname()

def store_agent_id(value: str):
    with open(_AGENT_ID_FILENAME, 'w') as f:
        f.write(value)
        f.write('\n')

def read_agent_id() -> 'str | None':
    if not os.path.exists(_AGENT_ID_FILENAME):
        return None

    with open(_AGENT_ID_FILENAME, 'r') as f:
        return f.readline()[:-1]