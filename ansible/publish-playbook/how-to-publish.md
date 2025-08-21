install ansible dependencies using the following command
```
ansible-galaxy install -r requirements.yml
```


this script assumes a few things about the host it will be deploying to:

- ansible user has passwordless login and passwordless sudo
- docker is installed and there is a docker group
- caddy is installed & running
- there is a group called restic 
- there is a files called `/etc/restic/backup-locations.txt` 


run the playbook using 
`ansible-playbook -i ./inventory.ini ./publish-playbook.yml`