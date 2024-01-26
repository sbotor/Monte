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

class CpuUsage(TypedDict):
    percentsUsed: 'list[float]'
    load: float

class MemoryUsage(TypedDict):
    available: int
    percentUsed: float
    swapAvailable: float
    swapPercentUsed: float

class SystemResources(TypedDict):
    cpu: CpuUsage
    memory: MemoryUsage

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

def initialize_monitoring():
    psutil.cpu_percent(0.0, True)
    psutil.getloadavg()

def get_system_info():
    initialize_monitoring()

    cpu = _get_cpu_info()
    memory = _get_memory_info()

    return SystemInfo(cpu=cpu, memory=memory)

def _get_cpu_usage(period: int):
    percents = psutil.cpu_percent(0.0, percpu=True)
    
    if period < 5:
        load_idx = 0
    elif period < 15:
        load_idx = 1
    else:
        load_idx = 2
    
    total_load = psutil.getloadavg()[load_idx]
    load = 100 * total_load / psutil.cpu_count()

    return CpuUsage(percentsUsed=percents, load=load)

def _get_memory_usage():
    memory = psutil.virtual_memory()
    swap = psutil.swap_memory()

    return MemoryUsage(
        available=memory.available,
        percentUsed=memory.percent,
        swapAvailable=swap.free,
        swapPercentUsed=swap.percent)

def get_system_resources(reporting_period: int):
    cpu = _get_cpu_usage(reporting_period)
    memory = _get_memory_usage()

    return SystemResources(cpu=cpu, memory=memory)