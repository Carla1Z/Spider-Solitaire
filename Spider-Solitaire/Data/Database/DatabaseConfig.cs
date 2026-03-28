using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

// Configuración central de la base de datos SQLite.
// Centralizar estos valores evita magic strings dispersos en el código.
namespace SpiderSolitaire.Data.Database
{
    /// <summary>
    /// Constantes de configuración para la base de datos SQLite local.
    /// SQLite es ideal para MAUI: liviano, sin servidor, multiplataforma.
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Nombre del archivo de base de datos.
        /// Se almacena en el directorio de datos de la app (FileSystem.AppDataDirectory).
        /// </summary>
        public const string DatabaseFileName = "spider_solitaire.db3";

        /// <summary>
        /// Flags de SQLite: lectura/escritura, crear si no existe, modo multi-hilo.
        /// </summary>
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        /// <summary>
        /// Ruta completa del archivo en el dispositivo.
        /// </summary>
        public static string DatabasePath =>
            System.IO.Path.Combine(
                FileSystem.AppDataDirectory,
                DatabaseFileName);
    }
}
