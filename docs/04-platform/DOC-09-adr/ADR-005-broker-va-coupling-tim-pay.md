# ADR-005 — Điều phối job và coupling TIM ↔ PAY

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-09-07 | soạn nháp SA (trợ lý) | **Proposed** — chờ PGD chốt |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Proposed** — gỡ **RK-04**, tiền đề của [ADR-011](ADR-011-lo-trinh-tach-service.md) W3 |
| **Date** | 2026-09-07 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · Dev (SH-010) · Ops/IT |
| **Informed** | QC · BA |
| **Liên quan** | ADR-001 (microservices) · ADR-010 (một DC, A/S) · ADR-011 (lộ trình wave) |

---

### Bối cảnh

DOC-08 để ngỏ ADR-005 với hai việc:

| DOC-08 | Nội dung |
|---|---|
| §4.2 dòng Job | *"Job T-15, T-7, N+3 — service riêng; broker ADR-005"* |
| §4.5 R-007 | *"**Saga** TIM→PAY trên microservices"* |
| §5 | *"Hàng đợi giữa service: **TBD** ADR-005"* |

ADR-011 W3 tách PAY thành service, nên phải chốt ADR-005 trước (**RK-04**).

### Phát hiện — coupling TIM ↔ PAY **không phải saga**

Soi code 2026-09-07 (`PayrollPeriodCommands`, `TimesheetImportCommands`):

| Chiều | Thực tế trong code | Loại |
|---|---|---|
| PAY → TIM | `PayrollPeriodCommands` đọc `FindPeriodByYmAsync(ym)`; nếu `Status != Closed` thì **từ chối tính lương** (PAY-FR-001) | **đọc guard đồng bộ** |
| TIM → PAY | `TimesheetImportCommands` gọi `IPayPeriodGate`; **cấm mở khoá kỳ công** nếu PAY đã chạy/chốt | **đọc guard đồng bộ** |

**Không có luồng ghi nào trải trên cả hai context.** PAY run chỉ ghi bảng `pay`; TIM close chỉ ghi bảng `tim`. Không có bước bù trừ, không có trạng thái trung gian cần cuộn ngược.

> **Hệ quả: giả định "saga TIM→PAY" trong DOC-08 §4.5 sai với hệ thống thật.** Đây là **hai lần đọc trạng thái chéo**, không phải giao dịch phân tán. Saga giải một bài toán không tồn tại.

Về job: hôm nay **không có `BackgroundService`/`IHostedService` nào**. T-15/T-7 và N+3 chạy bằng **endpoint được gọi từ ngoài** (`POST /v1/prb/jobs/reminders/run`, `POST /v1/lif/offboarding/jobs/nplus3-locks`). Bộ lập lịch nằm ngoài hệ thống.

### Quyết định *(đề xuất — chờ PGD)*

**1. Bỏ khái niệm saga TIM↔PAY.** Thay bằng **đọc trạng thái chéo qua API** khi PAY tách service (W3):

- PAY hỏi TIM: `GET /v1/tim/periods/{ym}` → dùng `status` làm guard.
- TIM hỏi PAY: `GET /v1/pay/periods/{ym}` → dùng làm guard mở khoá.
- Guard **fail-closed**: gọi lỗi hoặc quá hạn ⇒ **từ chối** thao tác, không đoán. Chốt công và chốt lương thà chặn còn hơn chạy sai.

**2. Không đưa message broker vào MVP.** Không Kafka/RabbitMQ/Azure Service Bus cho tới khi có luồng ghi thật sự phân tán. Lý do ở phần dưới.

**3. Job giữ mô hình "endpoint + bộ lập lịch ngoài"**, thêm hai ràng buộc:

- Chỉ chạy trên node **Active** (ADR-010 §5) — bộ lập lịch trỏ vào LBS, LBS chỉ bơm vào Active.
- Endpoint job phải **idempotent theo ngày** — gọi lại cùng ngày không sinh nhắc trùng. `LifAccessLockOutbox` đã có `idempotencyKey`; PRB reminder cần kiểm lại.

**4. Outbox giữ nguyên phạm vi hiện tại** — `PayExportOutbox`, `LeaveNotificationOutbox` phục vụ **hiệu ứng ra ngoài** (gửi mail, xuất file), không phải đồng bộ TIM↔PAY.

### Lý do

Ràng buộc quyết định là **năng lực vận hành** (DOC-14 A-01/R-01, ADR-010): một DC, chưa có đội ops. Thêm một broker là thêm một hệ thống trạng thái phải backup, giám sát, vá lỗi và khôi phục — trong khi bài toán nó giải (giao dịch phân tán) **chưa tồn tại** trong code.

Đọc guard đồng bộ trung thực hơn với nghiệp vụ: chốt lương **phải** biết chắc kỳ công đã chốt tại thời điểm chạy. Sự kiện bất đồng bộ đưa vào một cửa sổ trạng thái cũ mà chính nghiệp vụ này không chấp nhận.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **P — Đọc guard qua API, không broker** *(đề xuất)* | Đúng hình dạng coupling thật; không thêm hạ tầng; hợp năng lực ops | PAY phụ thuộc TIM lúc chạy — cần fail-closed + timeout rõ ràng |
| Q — Message broker + saga | Chuẩn microservices sách vở | **Giải bài toán không tồn tại**; thêm hệ thống phải vận hành; cửa sổ dữ liệu cũ trái nghiệp vụ chốt kỳ |
| R — TIM publish sự kiện, PAY giữ read model | Giảm coupling runtime | Vẫn cần broker; read model có thể cũ đúng lúc chốt lương — rủi ro cao nhất |
| S — Giữ đọc thẳng database chéo | Đơn giản nhất | **Loại** — phá hàng rào W1 vừa dựng, trái NFR-002 |

### Hệ quả

**Tích cực:** W3 hết chặn · không thêm hạ tầng phải vận hành · guard khớp ngữ nghĩa nghiệp vụ · DOC-08 §4.5 và §5 được sửa cho đúng thực tế.

**Tiêu cực:** PAY và TIM **coupling lúc chạy** — TIM sập thì không tính được lương. Chấp nhận được: không chốt công thì cũng không có gì để tính.

**Rủi ro:**

- **RK-05** — Chưa có timeout/retry policy cho guard chéo. Phải chốt ở W3, cùng lúc dựng Gateway.
- **RK-06** — Bộ lập lịch job nằm **ngoài** hệ thống và chưa được tài liệu hoá ở DOC-17. Ai bấm, bấm lúc nào, quan sát ra sao — còn trống.
- **RK-07** — Idempotency của `POST /v1/prb/jobs/reminders/run` chưa được kiểm; gọi hai lần có thể nhắc trùng (NFR-009 *"0 sót 0 trễ"* không nói gì về trùng).

### Tuân thủ & Tác động NFR

| NFR | Impact |
|-----|--------|
| NFR-001 | Guard chéo thêm một hop mạng khi PAY tách (W3) — đo lại 1000 dòng &lt; 5s sau W3 |
| NFR-002 | Không đổi — guard đọc **status kỳ**, không đọc số lương |
| NFR-005 | Không đổi — audit vẫn trong context ghi (DEC-ARC-024) |
| NFR-009 | RK-07 phải đóng trước go-live |
| NFR-012 | Không broker ⇒ không thêm thành phần cần A/S và backup (ADR-010) |

### DOC bị ảnh hưởng

| DOC | Thay đổi |
|-----|----------|
| DOC-08 §4.5 R-007 | Bỏ chữ "saga"; ghi là guard đọc chéo |
| DOC-08 §5 | *"Hàng đợi giữa service: TBD"* → **không dùng broker ở MVP** |
| DOC-12 | `GET /v1/tim/periods/{ym}` và `GET /v1/pay/periods/{ym}` trở thành **hợp đồng liên service**, không chỉ API nội bộ |
| DOC-17 | Thêm mục bộ lập lịch job: ai chạy, tần suất, giám sát (RK-06) |

### Câu hỏi mở

- **OQ-ARC-017** — Bộ lập lịch job dùng gì (cron hệ điều hành, Jenkins, hay Jarvis worker)? RK-06.
- **OQ-ARC-018** — Timeout/retry cho guard chéo TIM↔PAY: bao nhiêu, thử lại mấy lần? RK-05, chốt ở W3.

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | | ☐ Accepted · ☐ Rejected · ☐ Đổi phương án |
| SA | | | ☐ |
| Dev | | | ☐ |
