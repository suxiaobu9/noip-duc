# NO-IP DUC 自動變更 DNS IP

- `NOIP_HOSTNAME` 可以為多組，以 `,` 分開
  > domain1.example.com,domain2.example.com

## docker

```bash
docker run --name noip-duc -e NOIP_USERNAME='' -e NOIP_PASSWORD='' -e NOIP_HOSTNAME='' -e DELAY_MIN='5' -v /host/path:/app/data arisuokay/noip-duc:latest
```

## docker compose

```yml
version: "1"
name: noip-duc
services:
  noip-duc:
    image: arisuokay/noip-duc:latest
    container_name: noip-duc
    environment:
      - NOIP_USERNAME=
      - NOIP_PASSWORD=
      - NOIP_HOSTNAME=
      - DELAY_MIN=5
    volumes:
      - /host/path:/app/data
```

## build 指令

```ps1
docker build -t arisuokay/noip-duc .
# 需要變更版本
docker tag arisuokay/noip-duc:latest arisuokay/noip-duc:v1.1

docker push arisuokay/noip-duc:latest
# 需要變更版本
docker push arisuokay/noip-duc:v1.1
pause
```
