---
marp: true
theme: wctc
style: |
  @import 'wctc-theme.css';
---
![WCTC Logo](https://www.wctc.edu/_resources/images/waukesha_logo.svg)

# LINQ Performance: .NET 6 vs .NET 8
*Instructor: Mark McArthey*

---

## Introduction

- Overview of LINQ
	- LINQ (Language Integrated Query) is a powerful feature in .NET that allows developers to write type-safe, declarative queries for data sources.
	- In Entity Framework, LINQ is used to query and manipulate data in a database in a strongly typed, composable, and readable way.
- Importance in Entity Framework and repository CRUD operations

---

## LINQ in .NET 6

- Performance characteristics
- LINQ in .NET 6 is mature and stable, with a wide range of methods for querying and manipulating data.
	- However, some LINQ methods can be performance bottlenecks, especially when dealing with large datasets or complex queries.

---

## LINQ in .NET 8

- Performance improvements
	- .NET 8 introduces several performance improvements and new features for LINQ.
	- These improvements can lead to faster execution times, reduced memory usage, and more efficient query execution in Entity Framework.
- Not all methods have been improved. Key improvements include:
	- Enhanced performance for methods like `Where`, `Select`, `Sum`, `OrderBy`, and `ToList`.
	- Better memory efficiency in certain scenarios, such as reduced allocations in query execution.
	- Some methods may not see significant changes in performance.

---

## Benchmarking Methodology

- Test scenarios (Summing numbers)
	- We used BenchmarkDotNet, a powerful .NET library for benchmarking, to measure the performance of different LINQ methods.
	- Our test scenarios involved summing a large array of numbers using different methods: LINQ's `Sum`, manual summation, `Span<T>`, `Vector<T>`, and `SimdLinq`.
- Metrics to compare (Execution time, Memory usage)
- Runtimes: .NET 6, .NET 7, and .NET 8

---

## Benchmark Results: .NET 6

- Results of `MeasureLinqSum`
- Results of `MeasureManualSum`
- Results of `MeasureSpanSum`
- Results of `MeasureVectorizedSum`
- Results of `MeasureSimdLinqSum`

---

## Benchmark Results: .NET 8

- Improvements in `MeasureLinqSum`
- Changes in manual, Span, Vectorized, and SimdLinq sums
- Overall performance comparison

---

## Memory Utilization and Optimization

- **Manual Sum**: Low memory overhead, straightforward implementation.
- **LINQ Sum**: Higher abstraction, potential for increased memory allocation.
- **Span Sum**: Reduced memory allocations by operating on a slice of the array.
- **Vectorized Sum**: Utilizes SIMD for parallel processing, efficient for large datasets.
- **SimdLinq Sum**: Leverages SIMD through the SimdLinq library, optimized for performance.

---

## Stack vs Heap

- **Stack**
  - Used for static memory allocation.
  - Stores value types and references to objects in the heap.
  - Memory management is handled by the system: objects are automatically deallocated when they go out of scope.
  - Faster to allocate and deallocate.
- **Heap**
  - Used for dynamic memory allocation.
  - Stores instances of reference types.
  - Memory must be manually managed: objects need to be explicitly allocated and deallocated (though .NET's garbage collector helps with this).
  - Slower to allocate and deallocate, but allows for more flexibility in size and lifespan of objects.

---

## Value Types vs Reference Types

- In .NET, there are two main types of types: value types and reference types.
- Value types include basic types like `int`, `float`, `bool`, `struct`, and `enum`. They are stored directly in the stack or inline in containing types.
- Reference types include classes, interfaces, delegates, and arrays. They are stored in the heap, and a reference to their location is stored in the stack.
- The key difference between them is how they are stored and referenced in memory.

---

## Value Types in Memory

- When a value type is created, a single space in memory is allocated to store the value.
- Value types are stored in the stack, which is faster to allocate and deallocate.
- When you assign a value type to another, the system creates a separate copy of the value in memory.
- Example:
    ```csharp
    int number1 = 10;
    int number2 = number1;
    number2 = 20;
    // number1 is still 10
    ```
    In this example, changing `number2` does not affect `number1` because they are separate copies in memory.

---

## Reference Types in Memory

- When a reference type is created, two pieces of memory are allocated: one in the stack and one in the heap.
- The actual data is stored in the heap, and a reference to this data is stored in the stack.
- When you assign a reference type to another, both reference the same data in the heap. If you change the data through one reference, it is reflected when accessing through the other reference.
- Example:
    ```csharp
    var list1 = new List<int> { 1, 2, 3 };
    var list2 = list1;
    list2.Add(4);
    // list1 now contains 1, 2, 3, 4
    ```
    In this example, adding an element to `list2` also affects `list1` because they both reference the same data in the heap.

---

## Pointers, Stack, and Heap in C

- **Stack**
  - When you define a local variable in a function, it's allocated on the stack.
  - You can get its address using the `&` operator and store it in a pointer.
  - Example:
    ```c
    int number = 10; // Stored in the stack
    int* p = &number; // Pointer p points to the address of number
    ```
  - In this example, `number` is a local variable stored on the stack, and `p` is a pointer that holds the address of `number`.

---

## Pointers, Stack, and Heap in C

- **Heap**
  - When you allocate memory dynamically using functions like `malloc()`, it's allocated on the heap.
  - `malloc()` returns a pointer to the allocated memory.
  - Example:
    ```c
    int* p = malloc(sizeof(int)); // Allocates memory on the heap
    *p = 10; // Dereferences the pointer to store 10 in the allocated memory
    ```
  - In this example, `p` is a pointer that holds the address of the memory allocated on the heap.
  - Remember to free any memory you allocate on the heap with `free()` to prevent memory leaks.

---	

## Span<T> in .NET

- `Span<T>` is a new type in .NET that provides a lightweight, flexible slice of a sequence.
- It can improve performance by reducing memory allocations and providing efficient access to data.
- `Span<T>` is a stack-only type, meaning it can't be stored in heap-allocated objects, reducing the chance of memory leaks.
- It's particularly useful when working with arrays or strings, as it allows you to work with a portion of these data structures without creating a new copy.
- Example usage:
    ```csharp
    int[] numbers = new int[] { 1, 2, 3, 4, 5 };
    Span<int> slice = numbers.AsSpan().Slice(start: 1, length: 3);
    ```
    In this example, `slice` is a span that includes the second, third, and fourth elements of the `numbers` array.

---

## Vector<T> and SIMD in .NET

- `Vector<T>` is a type in .NET that can leverage SIMD (Single Instruction, Multiple Data) operations for parallel processing.
- SIMD operations can significantly speed up calculations by performing multiple operations at once.
- The length of a `Vector<T>` instance is hardware-dependent. On a system with 256-bit AVX2 support, a `Vector<float>` would contain 8 elements, while on a system with 128-bit SSE support, it would contain 4 elements.
- Example usage:
    ```csharp
    Vector<float> vector1 = new Vector<float>(1.0f);
    Vector<float> vector2 = new Vector<float>(2.0f);
    Vector<float> sum = Vector.Add(vector1, vector2);
    ```
    In this example, `sum` is a vector where each element is the sum of the corresponding elements in `vector1` and `vector2`.

---

## SimdLinq Library

- Extends LINQ with SIMD support for improved performance
	- `SimdLinq` is a library that extends LINQ with SIMD support for improved performance.
	- It provides SIMD-enabled versions of common LINQ methods, such as `Sum`, `Average`, and `Min`.

---

## Implications for Entity Framework

- Impact of LINQ performance on CRUD operations
	- The performance of LINQ can have a significant impact on the performance of Entity Framework and CRUD operations.
	- By optimizing LINQ queries and leveraging the improvements in .NET 8 and SIMD support, developers can build more efficient and performant data access layers.
- Optimizing queries with .NET 8 enhancements and SIMD support

---

## Conclusion

- **Performance Improvements**: .NET 8 introduces several performance improvements for LINQ, leading to faster execution times, reduced memory usage, and more efficient query execution in Entity Framework.
- **Key Enhancements**: Notable improvements include enhanced performance for methods like `Where`, `Select`, `Sum`, `OrderBy`, and `ToList`, and better memory efficiency in certain scenarios.
- **Impact on CRUD Operations**: These improvements can significantly impact the performance of CRUD operations in Entity Framework, enabling developers to build more efficient and performant data access layers.
- **Recommendations**: It's recommended to upgrade to .NET 8 to leverage these improvements. Developers should also optimize their LINQ queries and consider leveraging SIMD support for further performance gains.
