# Use LTS version of Node
FROM node:14

RUN npm -g install npm@latest

RUN mkdir -p /app

WORKDIR /app

COPY package.json /app
COPY package-lock.json /app

RUN npm ci --quiet --omit=dev --legacy-peer-deps

COPY . /app

EXPOSE 3010

CMD ["node", "keystone"]
