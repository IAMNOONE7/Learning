## Project Overview — Learning Python Testing

This project was created as a hands-on learning environment for mastering Python testing techniques.  
Each module and test file focuses on a different aspect of modern testing practices, demonstrating how to write clean, reliable, and maintainable tests using pytest and the Python standard library.

The project covers:

- Basic unit testing with pytest
- Testing arithmetic logic, error handling, and floating-point precision
- Using fixtures to provide reusable test data and setup
- Parametrization for generating multiple test cases efficiently
- Mocking external dependencies with Mock, MagicMock, AsyncMock
- Using side_effect to simulate exceptions, sequences of responses, and custom behaviors
- Testing async functions, async fixtures, and async dependencies
- Integration testing that exercises multiple modules together using real filesystem operations
- Marking tests (e.g., integration) for selective test runs

Each test file demonstrates one or more testing techniques in isolation.  
The goal of the project is not to produce a production application, but to serve as a structured, practical study environment for learning how to test Python code correctly and effectively.

---

### test_math_utils.py

- Basic pytest assertions:
  Simple "assert" statements verify that functions like add() and divide() return the expected results.

- Testing floating-point behavior:
  Uses math.isclose() to safely compare float values, avoiding precision issues with expressions like 0.1 + 0.2.

- Using pytest fixtures (@pytest.fixture):
  Demonstrates how fixtures provide reusable setup data (math_context) that tests automatically receive as arguments.

- Testing exceptions using pytest.raises:
  Confirms that divide() correctly raises a ValueError when dividing by zero.

- Parametrized tests (@pytest.mark.parametrize):
  Runs the same test function with multiple input/output combinations for is_prime(), reducing duplication and increasing coverage.


---

### test_string_utils.py 

- Mocking external dependencies:
  The test_download_text_mocked test uses unittest.mock.Mock and patch to replace the real requests.get() call with a fake response object. This prevents real HTTP requests and keeps the test fast and deterministic.

- Verifying mock behavior:
  The mock ensures the download_text() function correctly calls requests.get(), and assert_called_once_with verifies the exact URL used in the request.

- Simulating HTTP responses:
  The fake_response object mimics a real requests.Response by providing a .text attribute and a no-op raise_for_status() method.

- Testing functions in isolation:
  download_text() is tested without relying on network connectivity, focusing only on its logic.

- Simple functional testing:
  The test_word_count test checks correct behavior of word counting with different inputs, including empty strings and irregular spacing.

- No external side effects:
  Because the HTTP layer is mocked, and word_count uses pure logic, these tests run consistently on any machine without dependencies.


---

### test_file_processor.py — What This Test File Demonstrates

- Using MagicMock to simulate file-like objects:
  MagicMock provides magic methods such as __enter__, __exit__, and __iter__, which are required when testing code that uses "with file_obj as f:" and "for line in f:". A normal Mock would not work without extra setup.

- Testing context-manager behavior:
  The tests verify functions that open a file inside a "with" block. MagicMock automatically handles __enter__ and __exit__, allowing the test to control what the context manager returns.

- Simulating file iteration:
  By configuring fake_file.__enter__.return_value.__iter__.return_value = iter([...]),
  the test feeds specific lines to read_file_lines(), replicating how real file iteration works.

- Verifying output of higher-level logic:
  count_file_lines() is tested to ensure it counts lines correctly after going through the same MagicMock file simulation.

- Demonstrating why MagicMock is essential:
  MagicMock is the correct tool when the object under test is expected to behave like a file, context manager, or iterable. It simplifies testing complex interactions without relying on the file system.

---

### test_network.py

- Using side_effect to simulate exceptions:
  The first test configures mock_get.side_effect to raise an HTTPError. This allows testing error handling in fetch_data() without making real network calls.

- Testing retry-like behavior with side_effect sequences:
  By providing a list to side_effect, each call to requests.get() returns the next fake response. This simulates different server responses on successive calls and allows testing logic that depends on changing results.

- Using side_effect with a custom function:
  The third test defines fake_get(), a function that selectively returns a fake response or raises an error based on the URL argument. This demonstrates how side_effect can be used to implement dynamic, argument-based behavior in mocks.

---

### test_param_examples.py

- Basic parametrization:
  Tests like test_is_positive_basic show how @pytest.mark.parametrize allows
  running the same test multiple times with different values, eliminating repetitive code.

- Multi-argument parametrization:
  test_add_multiple_params demonstrates passing multiple values (a, b, expected)
  into a single test, enabling clear and compact testing of multiple scenarios.

- Readable parametrized IDs:
  test_word_count_with_ids uses pytest.param(..., id="name") to make test output
  clearer and easier to understand when many cases are executed.

- Parametrizing exceptions:
  In test_safe_divide_parametrized, a ValueError is expected only for one of the
  inputs. Parametrization mixes expected values and expected exceptions cleanly.

- Parametrized fixtures:
  The positive_number fixture contains a params list, causing pytest to run tests
  using this fixture once per value automatically.

- Parametrizing a full test class:
  The TestIsPositiveClass class shows that marks can be applied to a class,
  running every test method with each provided value.

- Indirect parametrization:
  test_indirect_user_fixture demonstrates how parameters are passed into a
  fixture (via request.param) when indirect=True is used, allowing the fixture
  to construct complex objects dynamically.

- Combining multiple parametrize decorators (Cartesian product):
  Tests like test_multiply_cartesian and test_combine_strings show how stacking
  decorators produces all combinations of parameters, useful for matrix-style
  testing of many input combinations.

- Broad demonstration of pytest's flexibility:
  This file collectively shows how parametrization, fixtures, marks, and class-level
  parameter injection allow extremely expressive, maintainable, and powerful tests.

---

### test_orders_integration.py

- Real integration testing without mocks:
  These tests verify that both the services and storage modules work together as a real system, instead of isolating functions through mocking.

- Using pytest's tmp_path fixture for temporary files:
  tmp_path provides a real directory on disk where the tests can safely create, read, and write files. This allows realistic file-based testing without polluting the project.

- Testing full data flow:
  The tests validate that the entire read/write workflow behaves correctly.

- Ensuring system correctness instead of isolated logic:
  These tests check the real behavior of the application when all layers interact, catching issues that unit tests alone cannot detect.

- Marked as integration tests:
  @pytest.mark.integration allows selective running of these tests using:
      pytest -m integration
      pytest -m "not integration"

---


### test_async_examples.py

- Basic async testing with pytest.mark.asyncio:
  Functions like test_async_add_basic show how to write async tests by marking them with @pytest.mark.asyncio, allowing pytest to await async functions naturally.

- Testing async behavior involving delays:
  The delayed_double function is tested without waiting in real time. Tests focus on correctness of results rather than actual elapsed time.

- Testing timeouts with asyncio.wait_for:
  test_wait_for_value_timeout verifies that long-running coroutines correctly raise asyncio.TimeoutError when wrapped in a timeout helper.

- Using async fixtures via pytest-asyncio:
  prepared_number demonstrates async setup logic inside fixtures. Tests using this fixture automatically await it and receive its result.

- AsyncMock for mocking async dependencies:
  fake_async_client fixture returns an AsyncMock, which supports awaitable methods such as client.get_data(). This allows testing async dependency interactions without real I/O.

- Verifying awaited calls:
  Tests assert that client.get_data was awaited with the correct arguments using assert_awaited_once_with and assert_any_await.

- Using side_effect with AsyncMock:
  test_fetch_and_upper_different_values shows how AsyncMock can return different values on each awaited call by using side_effect, enabling simulation of varied async responses.

- Realistic async workflow testing:
  The fetch_and_upper function is tested in isolation while its async dependency is replaced with AsyncMock, keeping the test fast, deterministic, and free from external services.

