import { rest } from "msw";
import { setupServer } from "msw/node";

const countData = {
  data: 0,
  message: "Unread Notification Count",
  serviceStatus: 200,
  status: true,
};

const server = setupServer(
  rest.get("https://localhost:8080", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json({ data: {} }));
  }),

  rest.get("https://localhost:8080/api/notification/count", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(countData));
  }),

  rest.get("*", (req, res, ctx) => {
    console.error(`Please add request handler for ${req.url.toString()}`);
    return res(
      ctx.status(500),
      ctx.json({ error: "You must add request handler." })
    );
  })
);

beforeAll(() => server.listen());
afterAll(() => server.close());
afterEach(() => server.resetHandlers());

export { server, rest };
