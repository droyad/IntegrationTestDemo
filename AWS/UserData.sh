#!/bin/bash
hostname=`curl http://169.254.169.254/latest/meta-data/public-hostname`
instanceid=`curl http://169.254.169.254/latest/meta-data/instance-id`
thumbprint=`ssh-keygen -E md5 -lf /etc/ssh/ssh_host_rsa_key.pub | grep -P -o "(?<=MD5:)[0-9a-f]+:[^ ]*"`

curl https://droyad.gq/api/machines --header "X-Octopus-ApiKey:%%apiKey%%" --data "%%registerRequest%%"