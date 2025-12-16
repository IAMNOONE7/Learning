from flask import Blueprint, render_template_string

ui_bp = Blueprint("ui", __name__)

HTML = """
<!doctype html>
<html>
<head>
  <meta charset="utf-8"/>
  <title>Books API UI</title>
  <style>
    body { font-family: sans-serif; max-width: 900px; margin: 20px auto; }
    textarea, input { width: 100%; padding: 8px; }
    textarea { height: 140px; }
    button { padding: 8px 12px; margin: 6px 0; }
    pre { background: #111; color: #0f0; padding: 12px; overflow: auto; }
    .row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
  </style>
</head>
<body>
  <h1>Books API UI</h1>

  <h2>GET /books</h2>
  <div class="row">
    <div>
      <label>Query string (optional)</label>
      <input id="qs" placeholder="page=1&page_size=10&sort_by=id&sort_dir=asc"/>
      <button onclick="getBooks()">Fetch</button>
    </div>
    <div>
      <label>Response</label>
      <pre id="outGet"></pre>
    </div>
  </div>

  <h2>POST /books</h2>
  <textarea id="postBody">{ "title": "New Book", "author": "Me", "year": 2025, "description": "..." }</textarea>
  <button onclick="postBook()">Create</button>
  <pre id="outPost"></pre>

  <h2>PUT /books/&lt;id&gt;</h2>
  <input id="putId" placeholder="Book ID (e.g. 1)"/>
  <textarea id="putBody">{ "title": "Updated title" }</textarea>
  <button onclick="putBook()">Update</button>
  <pre id="outPut"></pre>

  <h2>DELETE /books/&lt;id&gt;</h2>
  <input id="delId" placeholder="Book ID (e.g. 1)"/>
  <button onclick="delBook()">Delete</button>
  <pre id="outDel"></pre>

<script>
async function getBooks() {
  const qs = document.getElementById("qs").value.trim();
  const url = "/books" + (qs ? ("?" + qs) : "");
  const r = await fetch(url);
  document.getElementById("outGet").textContent =
    r.status + "\\n" + JSON.stringify(await r.json(), null, 2);
}

async function postBook() {
  const body = document.getElementById("postBody").value;
  const r = await fetch("/books", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body
  });
  const txt = await r.text();
  document.getElementById("outPost").textContent = r.status + "\\n" + txt;
}

async function putBook() {
  const id = document.getElementById("putId").value.trim();
  const body = document.getElementById("putBody").value;
  const r = await fetch("/books/" + id, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body
  });
  const txt = await r.text();
  document.getElementById("outPut").textContent = r.status + "\\n" + txt;
}

async function delBook() {
  const id = document.getElementById("delId").value.trim();
  const r = await fetch("/books/" + id, { method: "DELETE" });
  const txt = await r.text();
  document.getElementById("outDel").textContent = r.status + "\\n" + txt;
}
</script>
</body>
</html>
"""

@ui_bp.get("/ui")
def ui():
  return render_template_string(HTML)
