using Steamworks.Ugc;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using Result = Steamworks.Result;
using System.Linq;
using System;

namespace WorldBoxModLoader
{
    internal class UserModPublisher
    {
        public static void Publish(ModConstants modConstants)
        {
            string workshopPath = SaveManager.generateWorkshopPath(modConstants.ModName);
            string imagePath = Path.Combine(workshopPath, "icon.png");

            if (Directory.Exists(workshopPath))
                Directory.Delete(workshopPath, true);

            Directory.CreateDirectory(workshopPath);

            modConstants.ShouldPublish = false;
            UtilsInternal.UpdateModJson(modConstants.MetaLocation, modConstants);

            string[] modFiles = Directory.GetFiles(modConstants.MetaLocation, "*", SearchOption.AllDirectories);
            foreach (string modFile in modFiles)
            {
                Uri file = new Uri(modFile);
                // Must end in a slash to indicate folder
                Uri folder = new Uri(modConstants.MetaLocation);
                string relativePath =
                Uri.UnescapeDataString(
                    folder.MakeRelativeUri(file)
                        .ToString()
                        .Replace('/', Path.DirectorySeparatorChar)
                    );

                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(workshopPath, relativePath)));
                File.Copy(modFile, Path.Combine(workshopPath, relativePath), true);
            }

            Editor editor = Editor.NewCommunityFile.WithContent(workshopPath).WithTag("Mod").WithPreviewFile(imagePath)
                .WithDescription(modConstants.Description).WithTitle(modConstants.ModName).WithPublicVisibility();

            editor.SubmitAsync(null).ContinueWith(delegate (Task<PublishResult> taskResult)
            {
                if (taskResult.Status != TaskStatus.RanToCompletion)
                {
                    Debug.LogError("!RanToCompletion");
                    return;
                }

                PublishResult result = taskResult.Result;
                if (!result.Success)
                {
                    Debug.LogError("!result.Success");
                }

                if (result.NeedsWorkshopAgreement)
                {
                    Application.OpenURL("steam://url/CommunityFilePage/" + result.FileId);
                }

                if (result.Result != Result.OK)
                {
                    Debug.LogError(result.Result.ToString());
                }

                modConstants.ModID = result.FileId;
                UtilsInternal.UpdateModJson(modConstants.MetaLocation, modConstants);

            }, TaskScheduler.Default);
        }
        public static void Update(ModConstants modConstants)
        {
            string workshopPath = SaveManager.generateWorkshopPath(modConstants.ModName);
            string imagePath = Path.Combine(workshopPath, "icon.png");

            if (Directory.Exists(workshopPath))
                Directory.Delete(workshopPath, true);

            Directory.CreateDirectory(workshopPath);

            modConstants.ShouldUpdate = false;
            UtilsInternal.UpdateModJson(modConstants.MetaLocation, modConstants);

            string[] modFiles = Directory.GetFiles(modConstants.MetaLocation, "*", SearchOption.AllDirectories);
            foreach (string modFile in modFiles)
            {
                Uri file = new Uri(modFile);
                // Must end in a slash to indicate folder
                Uri folder = new Uri(modConstants.MetaLocation);
                string relativePath =
                Uri.UnescapeDataString(
                    folder.MakeRelativeUri(file)
                        .ToString()
                        .Replace('/', Path.DirectorySeparatorChar)
                    );

                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(workshopPath, relativePath)));
                File.Copy(modFile, Path.Combine(workshopPath, relativePath), true);
            }

            Editor editor = new Editor(modConstants.ModID).WithContent(workshopPath).WithTag("Mod").WithPreviewFile(imagePath)
                .WithDescription(modConstants.Description).WithTitle(modConstants.ModName).WithPublicVisibility();

            editor.SubmitAsync(null).ContinueWith(delegate (Task<PublishResult> taskResult)
            {
                if (taskResult.Status != TaskStatus.RanToCompletion)
                {
                    Debug.LogError("!RanToCompletion");
                    return;
                }

                PublishResult result = taskResult.Result;
                if (!result.Success)
                {
                    Debug.LogError("!result.Success");
                }

                if (result.NeedsWorkshopAgreement)
                {
                    Application.OpenURL("steam://url/CommunityFilePage/" + result.FileId);
                }

                if (result.Result != Result.OK)
                {
                    Debug.LogError(result.Result.ToString());
                }

                modConstants.ModID = result.FileId;
                UtilsInternal.UpdateModJson(modConstants.MetaLocation, modConstants);

            }, TaskScheduler.Default);
        }
    }
}
