import os
from flask import Flask
from dotenv import load_dotenv

from .db import db
from .routes.ui import ui_bp
from .routes.books import books_bp

def create_app() -> Flask:
    load_dotenv(override=False)

    app = Flask(__name__)

    # Flask-SQLAlchemy expects a URI, not DSN format
    app.config["SQLALCHEMY_DATABASE_URI"] = os.getenv("DATABASE_URL", "sqlite:///dev.db")
    app.config["SQLALCHEMY_TRACK_MODIFICATIONS"] = False

    db.init_app(app)
    app.register_blueprint(ui_bp)
    app.register_blueprint(books_bp)

    return app
