FROM node:14

ENV IN_DOCKER=true

RUN mkdir -p /tests

WORKDIR /tests

COPY package.json /tests
COPY package-lock.json /tests

RUN npm ci --quiet

COPY . /tests

CMD tail -f /dev/null