# Singleton Pattern

## Mô tả
Singleton đảm bảo một class chỉ có duy nhất một instance trong toàn bộ vòng đời ứng dụng và cung cấp một điểm truy cập toàn cục đến instance đó.

Pattern này thường được dùng cho các thành phần dùng chung như configuration, logger, cache manager, hoặc resource pool cần có trạng thái thống nhất.

## Ý tưởng cốt lõi
- Chỉ tạo một instance duy nhất.
- Cung cấp một điểm truy cập chung.
- Kiểm soát việc khởi tạo để tránh tạo bản sao mới.

## Cấu trúc tham gia
- Singleton class: tự quản lý instance của chính nó.
- Client: truy cập instance thông qua method hoặc property chung.

## Khi nào dùng
- Khi hệ thống thật sự cần một đối tượng duy nhất.
- Khi có trạng thái dùng chung cần đồng bộ.
- Khi muốn kiểm soát chặt tài nguyên dùng chung.

## Khi không nên dùng
- Khi chỉ là convenience shortcut cho shared state.
- Khi việc global state làm hệ thống khó test hoặc khó mở rộng.
- Khi có thể inject dependency thay vì dùng singleton.

## Ví dụ (Python)
Ví dụ dưới đây dùng `__new__` để đảm bảo chỉ có một instance.

```python
class SingletonConfig:
	_instance = None

	def __new__(cls, *args, **kwargs):
		if cls._instance is None:
			cls._instance = super().__new__(cls)
		return cls._instance

	def __init__(self):
		if not hasattr(self, "settings"):
			self.settings = {}

	def set(self, key, value):
		self.settings[key] = value

	def get(self, key, default=None):
		return self.settings.get(key, default)


a = SingletonConfig()
b = SingletonConfig()

a.set("env", "prod")
print(a is b)
print(b.get("env"))
```

### Giải thích ví dụ
- `__new__` kiểm soát việc tạo object.
- `_instance` lưu instance duy nhất.
- `__init__` chỉ khởi tạo state lần đầu.

## Quy trình hoạt động
1. Client gọi constructor hoặc method truy cập.
2. Class kiểm tra đã có instance hay chưa.
3. Nếu chưa có, tạo mới và lưu lại.
4. Nếu đã có, trả về instance cũ.

## Ưu điểm
- Đảm bảo chỉ có một instance.
- Dễ truy cập ở mọi nơi trong hệ thống.
- Phù hợp cho một số tài nguyên dùng chung.

## Nhược điểm
- Tạo global state, làm code khó test.
- Có thể che giấu dependency thật sự của class.
- Trong môi trường đa luồng, cần chú ý thread-safety nếu khởi tạo phức tạp.

## Phân biệt nhanh
- Singleton kiểm soát số lượng instance.
- Factory tạo object theo cách linh hoạt.
- Dependency Injection thường là lựa chọn tốt hơn nếu muốn test dễ hơn.

## Lỗi thường gặp
- Dùng singleton cho mọi thứ chỉ vì tiện.
- Lạm dụng singleton như global variable.
- Không xử lý đúng vấn đề thread-safe trong các ngữ cảnh concurrent.

## Checklist
- [ ] Có lý do thực sự để chỉ tồn tại một instance.
- [ ] Việc tạo instance được kiểm soát rõ ràng.
- [ ] Có thể test mà không phụ thuộc vào state rời rạc.
- [ ] Không dùng singleton để che giấu thiết kế chưa tốt.

## Tóm tắt ngắn
Singleton chỉ nên dùng khi thật sự cần đúng một instance duy nhất và bạn chấp nhận đánh đổi về global state và khả năng kiểm thử.

---

Người soạn: ArchReview AI — Singleton pattern.
