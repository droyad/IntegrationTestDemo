#!/bin/bash
hostname=`curl http://169.254.169.254/latest/meta-data/public-hostname`
instanceid=`curl http://169.254.169.254/latest/meta-data/instance-id`
thumbprint=`ssh-keygen -lf /etc/ssh/ssh_host_rsa_key.pub | egrep -o "[0-9a-f]+:[^ ]*"`

curl https://droyad.gq/api/machines --header "X-Octopus-ApiKey:API-WNKDGUJOY857GW3A0ID7WQHC0" --data "%%registerRequest%%"