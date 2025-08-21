

# how to deploy

1. copy and rename `inventory.example.ini` to `inventory.ini` & fill in the nessecary info
2. ensure passwordless login and passwordless sudo with the user defined in the `inventory.ini`  
3. copy and rename `variables.example.yml` to `variables.yml`
4. fill in the required date in `variables.yml` (random postgres password and a desired hostname)
5. run the playbook using `ansible-playbook -i ./inventory.ini ./publish-playbook.yml`


this script assumes a few things about the host it will be deploying to:

- docker is installed and there is a docker group
- caddy is installed, running and has a directory `/etc/sites/` that can contain caddyfiles that will automatically be imported
- there is a group called restic 
- there is a directory `/home/restic/backup-locations/` that can contain .txt files wich on each line a backup location.



