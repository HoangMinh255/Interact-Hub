## Required package

This implementation uses Azure Blob SDK:

```bash
dotnet add Application package Azure.Storage.Blobs
```

## --------------------Thêm StoryService , FileStorageService-------


## Files included

- `IFileStorageService`
- `FileStorageService`
- `IStoryService`
- `StoryService`
- `IStoryRepository`
- `StoryRepository`
- `CreateStoryDto`
- `UpdateStoryDto`
- `BlobStorageOptions`
- `Phase3ServiceExtensions`

## Notes

- `StoryService` currently treats the feed as stories of the current user for this first batch.
- Later we can expand the feed to friend-based stories using the friendship graph.
- `FileStorageService` is isolated and can be mocked in unit tests.

## Added files

- `StoriesController`
- `CreateStoryWithFileRequestDto`
- `StoryResponseDto`
- `StoryFeedQueryDto`
- `Phase4ApiExtensions`

## Endpoint summary

- `GET /api/stories/me`
- `GET /api/stories/feed?page=1&pageSize=10`
- `POST /api/stories`
- `POST /api/stories/upload`
- `PUT /api/stories/{id}`
- `PATCH /api/stories/{id}/hide`
- `DELETE /api/stories/{id}`

## Notes

- Controller is thin and delegates business rules to `IStoryService`.
- Blob upload is isolated in `IFileStorageService`.
- Response success paths are wrapped with `ApiResponse.Ok(...)` to match the existing AuthController style.

## --------------------Thêm ReportService , UserService-------

## Những gì đã có trong source
- `ReportService`
  - tạo report
  - xem report của tôi
  - xem pending report
  - review / resolve report
  - tạo notification cho reporter và chủ post

- `UserService`
  - lấy user by id / email / username
  - search user với pagination
  - update profile
  - upload avatar lên Azure Blob
  - deactivate / reactivate user

- Controller đi kèm
  - `ReportsController`
  - `UsersController`