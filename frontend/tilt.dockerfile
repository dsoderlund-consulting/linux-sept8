FROM docker.io/node:slim
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
COPY tilt.vite.config.ts vite.config.ts
CMD ["npm", "run", "dev"]