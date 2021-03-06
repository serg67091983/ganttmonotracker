﻿using System.IO;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using File = Google.Apis.Drive.v3.Data.File;

namespace GanttMonoTracker
{
	public class GDriveUploader
	{
		public bool Upload(GDriveCredentials cred, byte[] raw, string fileId, bool exists)
		{
			var service = new DriveService(new BaseClientService.Initializer
			{
				HttpClientInitializer = cred.Credential,
				ApplicationName = "GMT",
			});

			var body = new File { Name = fileId, Description = "Gantt Mono Tracker project", MimeType = "text/xml" };

			//manual
			//https://developers.google.com/drive/v3/web/manage-uploads

			FilesResource.CreateMediaUpload request;
			using (var stream = new MemoryStream(raw))
			{
				if (exists)
				{
					try
					{
						FilesResource.UpdateMediaUpload updateRequest = service.Files.Update(body, fileId, stream, "text/xml");
						var updateProcess = updateRequest.Upload();
						if (updateProcess.Status != Google.Apis.Upload.UploadStatus.Completed)
						{
							throw updateProcess.Exception;
						}
					}
					catch
					{
						exists = false;
					}
				}

				if(!exists)
				{
					request = service.Files.Create(
					body, stream, "text/xml");
					request.Fields = "id";

					/*{
					 "error": {
					  "errors": [
					   {
						"domain": "global",
						"reason": "insufficientPermissions",
						"message": "Insufficient Permission"
					   }
					  ],
					  "code": 403,
					  "message": "Insufficient Permission"

					 }
					}*/

					var process = request.Upload();
					if (process.Status != Google.Apis.Upload.UploadStatus.Completed)
					{
						throw process.Exception;
					}
				}
			}
			return true;
		}
	}
}
