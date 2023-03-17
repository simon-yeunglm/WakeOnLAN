#! /bin/bash
cd -- "$(dirname "$BASH_SOURCE")"
nc -4u -w0 192.168.1.111 4321 < WakeOnLAN.msg
