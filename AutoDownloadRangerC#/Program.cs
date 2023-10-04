using System;
using System.IO;

class Program
{
    static void Main()
    {
        int executableOnlyChoice = 2;
        Console.WriteLine("Bienvenue dans le programme de tri de fichiers!");
        Console.WriteLine("Le programme peut déplacer ou copier les fichiers depuis votre dossier de téléchargement ou un autre emplacement.");

        Console.WriteLine("Veuillez choisir le mode:");
        Console.WriteLine("1. Mode Classique (Rangement des téléchargements)");
        Console.WriteLine("2. Mode Étudiant");

        int modeChoice;
        while (!int.TryParse(Console.ReadLine(), out modeChoice) || (modeChoice != 1 && modeChoice != 2))
        {
            Console.WriteLine("Veuillez entrer 1 pour le Mode Classique ou 2 pour le Mode Étudiant.");
        }

        if (modeChoice == 1)
        {
            // Mode Classique (rangement des téléchargements)
            Console.WriteLine("Voulez-vous trier les fichiers depuis votre dossier de téléchargement ?");
            Console.WriteLine("1. Oui");
            Console.WriteLine("2. Non (spécifier un autre emplacement)");
            Console.WriteLine("3. Quitter");

            int useDownloadsFolderChoice;
            while (!int.TryParse(Console.ReadLine(), out useDownloadsFolderChoice) || (useDownloadsFolderChoice != 1 && useDownloadsFolderChoice != 2 && useDownloadsFolderChoice != 3))
            {
                Console.WriteLine("Veuillez entrer 1 pour trier les fichiers depuis votre dossier de téléchargement, 2 pour spécifier un autre emplacement, ou 3 pour quitter.");
            }

            if (useDownloadsFolderChoice == 3)
            {
                Console.WriteLine("L'application est en train de se fermer. Appuyez sur une touche pour quitter...");
                Console.ReadKey();
                return;
            }

            string sourceFolderPath = null;
            if (useDownloadsFolderChoice == 2)
            {
                Console.WriteLine("Veuillez entrer le chemin du dossier source:");
                sourceFolderPath = Console.ReadLine();

                if (!Directory.Exists(sourceFolderPath))
                {
                    Console.WriteLine("Le dossier source spécifié n'existe pas.");
                    Console.WriteLine("L'application est en train de se fermer. Appuyez sur une touche pour quitter...");
                    Console.ReadKey();
                    return;
                }
            }

            Console.WriteLine("Voulez-vous déplacer ou copier les fichiers ?");
            Console.WriteLine("1. Déplacer");
            Console.WriteLine("2. Copier");
            Console.WriteLine("3. Quitter");

            int moveOrCopyChoice;
            while (!int.TryParse(Console.ReadLine(), out moveOrCopyChoice) || (moveOrCopyChoice != 1 && moveOrCopyChoice != 2 && moveOrCopyChoice != 3))
            {
                Console.WriteLine("Veuillez entrer 1 pour Déplacer, 2 pour Copier ou 3 pour Quitter.");
            }

            if (moveOrCopyChoice == 3)
            {
                Console.WriteLine("L'application est en train de se fermer. Appuyez sur une touche pour quitter...");
                Console.ReadKey();
                return;
            }

            string destinationFolderPath = useDownloadsFolderChoice == 1
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads"
                : sourceFolderPath;

            string[] files = Directory.GetFiles(destinationFolderPath);

            if (files.Length == 0)
            {
                Console.WriteLine("Aucun fichier à trier trouvé dans le dossier spécifié.");
                Console.WriteLine("Appuyez sur une touche pour quitter...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Le logiciel est en train de trier les fichiers. Patientez...");

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string fileName = Path.GetFileNameWithoutExtension(file);

                if (extension != null)
                {
                    if (executableOnlyChoice == 1 && !extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string destinationFolder;

                    if (extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        destinationFolder = GetDestinationFolder(destinationFolderPath, extension, fileName);
                    }
                    else if (IsImage(extension))
                    {
                        destinationFolder = GetImageDestinationFolder(destinationFolderPath, extension);
                    }
                    else
                    {
                        destinationFolder = GetDestinationFolder(destinationFolderPath, extension, "Misc_" + extension.Substring(1));
                    }

                    if (!string.IsNullOrEmpty(destinationFolder))
                    {
                        DateTime downloadDate = File.GetLastWriteTime(file);
                        string yearFolder = Path.Combine(destinationFolder, downloadDate.Year.ToString());
                        Directory.CreateDirectory(yearFolder);

                        string monthFolder = Path.Combine(yearFolder, downloadDate.ToString("MM-MMMM"));
                        Directory.CreateDirectory(monthFolder);

                        string destinationFilePath = Path.Combine(monthFolder, Path.GetFileName(file));

                        if (moveOrCopyChoice == 1)
                        {
                            File.Move(file, destinationFilePath);
                        }
                        else
                        {
                            File.Copy(file, destinationFilePath);
                        }
                    }
                }
            }

            Console.WriteLine("Travail terminé. Bonne journée!");
            Console.WriteLine("Appuyez sur une touche pour quitter...");
            Console.ReadKey();
        }
        else if (modeChoice == 2)
        {
            string studentInfoFilePath = "student_info.txt";

            if (File.Exists(studentInfoFilePath))
            {
                Console.WriteLine("Un utilisateur existe déjà. Voulez-vous charger l'utilisateur existant ou en créer un nouveau ?");
                Console.WriteLine("1. Charger l'utilisateur existant");
                Console.WriteLine("2. Créer un nouvel utilisateur");

                int loadOrCreateChoice;
                while (!int.TryParse(Console.ReadLine(), out loadOrCreateChoice) || (loadOrCreateChoice != 1 && loadOrCreateChoice != 2))
                {
                    Console.WriteLine("Veuillez entrer 1 pour charger l'utilisateur existant ou 2 pour créer un nouvel utilisateur.");
                }

                string lastName = "";
                string firstName = "";
                string studentClass = "";

                if (loadOrCreateChoice == 1)
                {
                    Console.WriteLine("Chargement de l'utilisateur existant...");

                    string[] userInfoLines = File.ReadAllLines(studentInfoFilePath);

                    if (userInfoLines.Length >= 3)
                    {
                        lastName = userInfoLines[0].Split(':')[1].Trim();
                        firstName = userInfoLines[1].Split(':')[1].Trim();
                        studentClass = userInfoLines[2].Split(':')[1].Trim();

                        Console.WriteLine("Utilisateur chargé avec succès!");
                        Console.WriteLine($"Nom : {lastName}");
                        Console.WriteLine($"Prénom : {firstName}");
                        Console.WriteLine($"Classe : {studentClass}");
                    }
                }
                else if (loadOrCreateChoice == 2)
                {
                    Console.WriteLine("Création d'un nouvel utilisateur:");
                    Console.Write("Nom : ");
                    lastName = Console.ReadLine();
                    Console.Write("Prénom : ");
                    firstName = Console.ReadLine();
                    Console.Write("Classe : ");
                    studentClass = Console.ReadLine();

                    using (StreamWriter writer = new StreamWriter(studentInfoFilePath))
                    {
                        writer.WriteLine($"Nom : {lastName}");
                        writer.WriteLine($"Prénom : {firstName}");
                        writer.WriteLine($"Classe : {studentClass}");
                    }

                    Console.WriteLine("Informations de l'utilisateur enregistrées avec succès!");
                }

                // Créer un répertoire pour la classe de l'élève
                string classDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Cours", studentClass);

                if (!Directory.Exists(classDirectory))
                {
                    Directory.CreateDirectory(classDirectory);
                }

                Console.WriteLine("Veuillez entrer le chemin de votre dossier de cours :");
                string courseFolderPath = Console.ReadLine();

                if (Directory.Exists(courseFolderPath))
                {
                    string[] courseFiles = Directory.GetFiles(courseFolderPath);

                    foreach (string courseFile in courseFiles)
                    {
                        string fileExtension = Path.GetExtension(courseFile);
                        if (!IsValidFileExtension(fileExtension))
                        {
                            Console.WriteLine($"Le fichier {Path.GetFileName(courseFile)} a une extension non prise en charge. Ignoré.");
                            continue;
                        }

                        string[] fileNameParts = Path.GetFileNameWithoutExtension(courseFile).Split('_');
                        if (fileNameParts.Length < 3)
                        {
                            Console.WriteLine($"Impossible de déterminer les détails du cours à partir du nom du fichier : {Path.GetFileName(courseFile)}. Ignoré.");
                            continue;
                        }

                        string courseName = fileNameParts[0].Trim();
                        string sequence = fileNameParts[1].Trim();
                        string courseType = fileNameParts[2].Trim();

                        // Utiliser uniquement le nom de fichier de base pour le nouveau nom
                        string newFileName = Path.GetFileNameWithoutExtension(courseFile)
                            .Replace(courseName + "_", "")
                            .Replace(sequence + "_", "")
                            .Replace(courseType + "_", "");

                        string classCourseDirectory = Path.Combine(classDirectory, courseName);
                        string sequenceDirectory = Path.Combine(classCourseDirectory, sequence);
                        string typeDirectory = Path.Combine(sequenceDirectory, courseType);

                        Directory.CreateDirectory(typeDirectory);

                        string destinationFilePath = Path.Combine(typeDirectory, newFileName + fileExtension);
                        File.Move(courseFile, destinationFilePath);

                        Console.WriteLine($"Le fichier {Path.GetFileName(courseFile)} a été traité avec succès.");
                    }
                }
                else
                {
                    Console.WriteLine("Le dossier de cours spécifié n'existe pas.");
                }
            }
            else
            {
                Console.WriteLine("Aucun utilisateur n'existe. Créez un nouvel utilisateur:");

                Console.Write("Nom : ");
                string lastName = Console.ReadLine();
                Console.Write("Prénom : ");
                string firstName = Console.ReadLine();
                Console.Write("Classe : ");
                string studentClass = Console.ReadLine();

                using (StreamWriter writer = new StreamWriter(studentInfoFilePath))
                {
                    writer.WriteLine($"Nom : {lastName}");
                    writer.WriteLine($"Prénom : {firstName}");
                    writer.WriteLine($"Classe : {studentClass}");
                }

                // Créer un répertoire pour la classe de l'élève
                string classDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), studentClass);

                if (!Directory.Exists(classDirectory))
                {
                    Directory.CreateDirectory(classDirectory);
                }

                Console.WriteLine("Nouvel utilisateur créé avec succès!");

                Console.WriteLine("Veuillez entrer le chemin de votre dossier de cours :");
                string courseFolderPath = Console.ReadLine();

                if (Directory.Exists(courseFolderPath))
                {
                    string[] courseFiles = Directory.GetFiles(courseFolderPath);

                    foreach (string courseFile in courseFiles)
                    {
                        string fileExtension = Path.GetExtension(courseFile);
                        if (!IsValidFileExtension(fileExtension))
                        {
                            Console.WriteLine($"Le fichier {Path.GetFileName(courseFile)} a une extension non prise en charge. Ignoré.");
                            continue;
                        }

                        string[] fileNameParts = Path.GetFileNameWithoutExtension(courseFile).Split('_');
                        if (fileNameParts.Length < 3)
                        {
                            Console.WriteLine($"Impossible de déterminer les détails du cours à partir du nom du fichier : {Path.GetFileName(courseFile)}. Ignoré.");
                            continue;
                        }

                        string courseName = fileNameParts[0];
                        string sequence = fileNameParts[1];
                        string courseType = fileNameParts[2];

                        // Utiliser uniquement le nom de fichier de base pour le nouveau nom
                        string newFileName = Path.GetFileNameWithoutExtension(courseFile)
                            .Replace(courseName + "_", "")
                            .Replace(sequence + "_", "")
                            .Replace(courseType + "_", "");

                        string classCourseDirectory = Path.Combine(classDirectory, courseName);
                        string sequenceDirectory = Path.Combine(classCourseDirectory, sequence);
                        string typeDirectory = Path.Combine(sequenceDirectory, courseType);

                        Directory.CreateDirectory(typeDirectory);

                        string destinationFilePath = Path.Combine(typeDirectory, newFileName + fileExtension);
                        File.Move(courseFile, destinationFilePath);

                        Console.WriteLine($"Le fichier {Path.GetFileName(courseFile)} a été traité avec succès.");
                    }
                }
                else
                {
                    Console.WriteLine("Le dossier de cours spécifié n'existe pas.");
                }
            }

            Console.WriteLine("Appuyez sur une touche pour quitter...");
            Console.ReadKey();
        }
    }

    static string GetDestinationFolder(string destinationFolderPath, string extension, string fileName)
    {
        if (extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
        {
            if (IsGameInstaller(fileName))
            {
                return Path.Combine(destinationFolderPath, "App_game_installer");
            }
            else if (IsCodingInstaller(fileName))
            {
                return Path.Combine(destinationFolderPath, "Coding_installer");
            }
            else if (IsPortableApp(fileName))
            {
                return Path.Combine(destinationFolderPath, "Portable_apps");
            }
        }
        else
        {
            return Path.Combine(destinationFolderPath, "Misc_" + extension.Substring(1));
        }

        return Path.Combine(destinationFolderPath, "MiscExecutable");
    }

    static string GetImageDestinationFolder(string destinationFolderPath, string extension)
    {
        return Path.Combine(destinationFolderPath, "MiscImg", extension.Substring(1));
    }

    static bool IsImage(string extension)
    {
        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".ico" };
        return Array.Exists(imageExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }

    static bool IsGameInstaller(string fileName)
    {
        return fileName.Contains("Steam", StringComparison.OrdinalIgnoreCase) ||
               fileName.Contains("XboxInstaller", StringComparison.OrdinalIgnoreCase) ||
               fileName.Contains("Epic Games", StringComparison.OrdinalIgnoreCase);
    }

    static bool IsCodingInstaller(string fileName)
    {
        return fileName.StartsWith("vs_", StringComparison.OrdinalIgnoreCase) ||
               fileName.Contains("JetBrains", StringComparison.OrdinalIgnoreCase) ||
               fileName.Contains("IntelliJ", StringComparison.OrdinalIgnoreCase) ||
               fileName.Contains("Notepad++", StringComparison.OrdinalIgnoreCase);
    }

    static bool IsPortableApp(string fileName)
    {
        return fileName.Contains("Portable", StringComparison.OrdinalIgnoreCase);
    }

    static bool IsValidFileExtension(string extension)
    {
        string[] validExtensions = { ".pdf", ".docx", ".doc", ".txt" };
        return Array.Exists(validExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }
}
