import { Link } from "@remix-run/react";

export default function Sidebar() {
  return (
    <div className="border-r w-80 h-full">
      <ul>
        <li>
          <a
            target="_blank"
            href="https://remix.run/tutorials/jokes"
            rel="noreferrer"
          >
            Deep Dive Jokes App Tutorial
          </a>
        </li>
        <li>
          <a target="_blank" href="https://remix.run/docs" rel="noreferrer">
            Remix Docs
          </a>
        </li>
        <li>
          <Link to="private">Private</Link>
        </li>
      </ul>
    </div>
  );
}
