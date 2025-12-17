from django_filters.rest_framework import DjangoFilterBackend
from rest_framework.filters import SearchFilter, OrderingFilter
from rest_framework.viewsets import ModelViewSet
from .models import Book
from .serializers import BookSerializer

class BookViewSet(ModelViewSet):
    queryset = Book.objects.all().order_by("id")
    serializer_class = BookSerializer
    # Enables filtering/search/sorting
    filter_backends = [DjangoFilterBackend, SearchFilter, OrderingFilter]

    # Exact-match filtering: /books/?author=....
    filterset_fields = ["author", "year"]

    # Search (LIKE / ILIKE style): /books/?search=clean
    search_fields = ["title", "author", "description"]

    # Ordering: /books/?ordering=author
    ordering_fields = ["id", "title", "author", "year"]
    ordering = ["id"]