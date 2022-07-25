FROM node:16 as builder

ENV NODE_ENV=production

# utils
WORKDIR /app/utils

COPY ./utils/package.json .

RUN npm install

COPY ./utils .

# launcher
WORKDIR /app/launcher

COPY ./launcher/package.json .

RUN npm install

COPY ./launcher .

# build
WORKDIR /app/utils

RUN node config-init.js prod ../launcher

WORKDIR /app/launcher

RUN npm run build