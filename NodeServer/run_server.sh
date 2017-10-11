#!/bin/bash
# A BASH script that restarts the server whenever there's a crash

until (node server.js); do
    echo "NetworkIt Server crashed with exit code $?.  Respawning.." >&2
    sleep 1
done
