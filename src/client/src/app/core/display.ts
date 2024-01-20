const formatMemoryCore = (value: number): [number, string] => {
  if (value < 1_000) {
    return [value, ''];
  }

  if (value < 1_000_000) {
    return [value / 1_000, ' kB'];
  }

  if (value < 1_000_000_000) {
    return [value / 1_000_000, ' MB'];
  }

  if (value < 1_000_000_000_000) {
    return [value / 1_000_000_000, ' GB'];
  }

  return [value / 1_000_000_000_000, ' TB'];
}

export const formatMemoryDisplay = (value: number) => {
  const [scaled, unit] = formatMemoryCore(value);
  return scaled.toFixed(1) + unit;
}
