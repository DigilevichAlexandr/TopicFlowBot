# TopicFlowBot Mini App

Production-ready MVP Telegram Mini App with React + ASP.NET Core + PostgreSQL.

## Folder structure

```text
.
├── frontend
│   ├── src
│   │   ├── App.tsx
│   │   ├── api.ts
│   │   ├── index.css
│   │   ├── main.tsx
│   │   ├── store.ts
│   │   ├── telegram.ts
│   │   └── types.ts
│   ├── .env.example
│   ├── package.json
│   ├── postcss.config.js
│   ├── tailwind.config.js
│   └── vite.config.ts
├── backend
│   ├── src
│   │   ├── TopicFlowBot.Api
│   │   │   ├── Controllers
│   │   │   ├── Program.cs
│   │   │   └── RequestLoggingMiddleware.cs
│   │   ├── TopicFlowBot.Application
│   │   ├── TopicFlowBot.Domain
│   │   └── TopicFlowBot.Infrastructure
│   ├── tests/TopicFlowBot.Tests
│   └── Dockerfile
├── docker-compose.yml
└── .env.example
```

## Setup

1. Copy env files:
   - `.env.example` -> `.env`
   - `frontend/.env.example` -> `frontend/.env`
2. Set real `TELEGRAM_BOT_TOKEN`.
3. Start DB + backend:
   - `docker compose up --build`
4. Start frontend:
   - `cd frontend`
   - `npm install`
   - `npm run dev`
5. Open frontend URL in Telegram Mini App settings.

## API endpoints

- `POST /auth/telegram` - validates Telegram `initData` hash on backend.
- `GET /topics?query=` - search topics.
- `POST /favorites` - add favorite.
- `GET /favorites/{userId}` - fetch user favorites.

## Security notes

- Backend validates `initData` with HMAC-SHA256 according to Telegram WebApp rules.
- Client-side user payload is never trusted directly.
- All operations requiring user identity go through backend auth endpoint.

## Deploy: Render + Vercel

### Backend on Render

1. Push repository to GitHub.
2. In Render, create Blueprint deploy using `render.yaml` from repository root.
3. Set secret env var `Telegram__BotToken` in Render dashboard.
4. Render will create PostgreSQL and inject `ConnectionStrings__Postgres`.
5. After deploy, copy API URL like `https://topicflowbot-api.onrender.com`.

### Frontend on Vercel

1. In Vercel, import `frontend` directory as a project.
2. Add env var `VITE_API_BASE_URL` with Render API URL.
3. Deploy project.
