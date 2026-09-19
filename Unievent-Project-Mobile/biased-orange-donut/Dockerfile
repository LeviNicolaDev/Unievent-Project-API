FROM node:22-alpine

WORKDIR /app

ENV CI=1

COPY package*.json ./
RUN npm ci

COPY . .

EXPOSE 8081 19000 19001 19002

CMD ["npx", "expo", "start", "--web", "--host", "lan", "--port", "8081"]
