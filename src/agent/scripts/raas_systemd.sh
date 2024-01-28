#!/bin/bash
# This script is confirmed to work on Ubuntu 20.04 LTS. It should work on other systems supporting systemd as well, however, it is not guaranteed.

# Set python path
export PYTHONPATH=/usr/bin/python3

# Create /monte directory in /usr/local/share
sudo mkdir -p /usr/local/share/Monte

# Copy files to the system files
sudo cp -r ../venv ../config.py ../config.yaml ../logger.py ../main.py ../monitoring.py ../monte_client.py ../utils.py /usr/local/share/Monte

# Create the virtual environment
source /usr/local/share/Monte/venv/bin/activate

# Create the .service file
sudo tee /etc/systemd/system/monte.service > /dev/null <<EOF
[Unit]
Description=Monte Agent Service
After=multi-user.target

[Service]
Type=simple
Restart=always
User=$USER
ExecStart=/usr/local/share/Monte/venv/bin/python3 /usr/local/share/Monte/main.py --config production
WorkingDirectory=/usr/local/share/Monte

[Install]
WantedBy=multi-user.target
EOF

# Reload the systemd daemon
sudo systemctl daemon-reload

# Enable the service
sudo sudo systemctl enable monte.service

# Start the service
sudo systemctl start monte.service
