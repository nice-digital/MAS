# Runs functional tests via Docker

docker-compose down --remove-orphans && docker-compose rm -vf
docker-compose build && docker-compose up -d
winpty docker-compose exec tests bash