# Azure Blob Storage Setup Guide

## Tổng quan
Hướng dẫn này giúp bạn cấu hình Azure Blob Storage để upload media trong InteractHub.

## Yêu cầu tiên quyết

### Backend (.NET)
1. Cài đặt package Azure.Storage.Blobs:
```bash
cd backend
dotnet add Api/Api.csproj package Azure.Storage.Blobs
```

### Frontend (React/TypeScript)
Đã cài đặt: `@azure/storage-blob`

## Cấu hình Azure Blob Storage

### 1. Tạo Azure Storage Account
1. Truy cập [Azure Portal](https://portal.azure.com)
2. Tạo một **Storage Account** mới
3. Ghi lại:
   - **Storage Account Name** (ví dụ: `myinteracthubstorage`)
   - **Access Key** (Primary hoặc Secondary)

### 2. Tạo Container
1. Vào Storage Account vừa tạo
2. Chọn **Containers** → **Create Container**
3. Đặt tên: `media`
4. Chọn **Public access level**: **Container (anonymous read access for containers and blobs)**

### 3. Lấy Connection String
1. Trong Storage Account, chọn **Access keys**
2. Copy **Connection string** (Primary hoặc Secondary)
3. Định dạng sẽ là: `DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=...;EndpointSuffix=core.windows.net`

## Cấu hình Backend

### 1. Cập nhật `appsettings.json`
```json
{
  "AzureBlob": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net",
    "ContainerName": "media",
    "LocalStoragePath": "uploads"
  }
}
```

### 2. Cấu hình cho môi trường khác nhau
Cập nhật các file sau nếu muốn sử dụng Azure trên tất cả môi trường:
- `appsettings.Development.json` (Development)
- `appsettings.Staging.json` (Staging)
- `appsettings.Production.json` (Production)

**Lưu ý**: Nếu `ConnectionString` trống, hệ thống sẽ sử dụng local storage (thư mục `uploads/`)

## Cách sử dụng

### Upload Media
1. **Trên Frontend**:
   - User chọn file (ảnh/video) từ form
   - Nhấn nút "Đăng bài"
   - Frontend gọi API: `POST /api/post/upload-media`
   - Server upload file lên Azure Blob Storage
   - Trả về URL của file

2. **Trên Backend**:
   - Endpoint: `POST /api/post/upload-media`
   - Accept: `multipart/form-data`
   - Response:
   ```json
   {
     "message": "Upload file thành công!",
     "data": [
       {
         "BlobName": "unique-name.jpg",
         "MediaUrl": "https://myaccount.blob.core.windows.net/media/unique-name.jpg",
         "FileName": "original-name.jpg",
         "ContentType": "image/jpeg"
       }
     ]
   }
   ```

### Tạo Post với Media
- Endpoint: `POST /api/post`
- Body:
```json
{
  "Content": "Bài viết của tôi",
  "Visibility": 0,
  "Media": [
    {
      "MediaUrl": "https://myaccount.blob.core.windows.net/media/unique-name.jpg",
      "MediaType": 0
    }
  ],
  "Hashtags": ["tag1", "tag2"]
}
```

### Cập nhật Post
- Endpoint: `PUT /api/post/{postId}`
- Body: Tương tự Create Post

## Tính năng

✅ Upload ảnh (JPG, PNG, GIF, WebP)
✅ Upload video (MP4, WebM, etc.)
✅ Hỗ trợ Azure Blob Storage
✅ Fallback về local storage khi không có Azure
✅ Tự động sinh unique filename
✅ Delete file khi post bị xóa

## Xử lý lỗi

### Lỗi: "Connection string không hợp lệ"
- Kiểm tra chuỗi kết nối trong `appsettings.json`
- Đảm bảo copy đúng từ Azure Portal

### Lỗi: "Container không tồn tại"
- Tạo container tên `media` trong Storage Account
- Kiểm tra tên container trong cấu hình

### Lỗi: "Quyền truy cập bị từ chối"
- Kiểm tra Access Level của container (phải là Public)
- Hoặc cấp quyền trong Shared Access Signature (SAS)

## Bảo mật

⚠️ **Khuyến cáo**:
1. Không commit Connection String vào Git
2. Sử dụng Azure Key Vault cho production
3. Sử dụng SAS token với thời gian hết hạn ngắn
4. Validate file type trên server
5. Giới hạn kích thước file upload

## Tham khảo

- [Azure Storage Documentation](https://docs.microsoft.com/en-us/azure/storage/)
- [Azure Storage Blobs SDK for .NET](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/storage/Azure.Storage.Blobs)
- [Azure Storage Blobs SDK for JavaScript](https://github.com/Azure/azure-sdk-for-js/tree/main/sdk/storage/storage-blob)
