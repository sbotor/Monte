from pathlib import Path
import socket
import os

_AGENT_ID_FILENAME = 'monteid'

def extract_hostname():
    return socket.gethostname()

def store_agent_id(path: str, value: str):
    path = Path(path, _AGENT_ID_FILENAME)
    with open(path, 'w') as f:
        f.write(value)
        f.write('\n')

def read_agent_id(path: str) -> 'str | None':
    path = Path(path, _AGENT_ID_FILENAME)
    if not os.path.exists(path):
        return None

    with open(path, 'r') as f:
        return f.readline()[:-1]