import psutil
from typing import TypedDict

class CpuInfo(TypedDict):
    logicalCount: int
    physicalCount: int
    minFreq: float
    maxFreq: float

class MemoryInfo(TypedDict):
    total: int
    swap: int

class SystemInfo(TypedDict):
    cpu: CpuInfo
    memory: MemoryInfo

def _get_cpu_info():
    logical_count = psutil.cpu_count(logical=True)
    physical_count = psutil.cpu_count(logical=False)
    
    freq = psutil.cpu_freq()
    min_freq = freq.min
    max_freq = freq.max

    return CpuInfo(
        logicalCount=logical_count,
        physicalCount=physical_count,
        minFreq=min_freq,
        maxFreq=max_freq)

def _get_memory_info():
    total = psutil.virtual_memory().total
    swap = psutil.swap_memory().total

    return MemoryInfo(total=total, swap=swap)

def get_system_info():
    cpu = _get_cpu_info()
    memory = _get_memory_info()

    return SystemInfo(cpu=cpu, memory=memory)