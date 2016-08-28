using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SoftServe.Helpers
{
    public class SoftServeDB : SQLiteConnection
    {
        public SoftServeDB(string path) : base(path)
        {
            CreateTable<User>();
            CreateTable<Permission>();
            CreateTable<Song>();
        }

        /// <summary>
        /// See if user exists and proper credentials are provided
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="passcode">User's passcode</param>
        /// <returns>True if authentication succeeded</returns>
        public bool AuthenticateUser(string username, string passcode)
        {
            return Table<User>().Any(user=>user.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase) && user.Passcode.Equals(passcode));
        }

        public bool TryCreateUser(string username, string passcode, out int id)
        {
            if (Table<User>().Any(user => user.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase)))
            {
                id = -1;
                return false;
            }
            else
            {
                id = Insert(new User() {UserName = username, Passcode = passcode});
                return true;
            }
        }

        /// <summary>
        /// A user object
        /// </summary>
        public class User
        {
            /// <summary>
            /// Automatically assigned id for the user
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int UserId { get; private set; }

            /// <summary>
            /// Visible user account name
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// Secret (and fairly insecure) user passcode
            /// </summary>
            public string Passcode { get; set; }

            public override string ToString()
            {
                return UserName;
            }
        }

        /// <summary>
        /// Permissions object
        /// </summary>
        public class Permission
        {
            /// <summary>
            /// Automatically assigned identifer for this object
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int PermissionId { get; private set; }

            /// <summary>
            /// User id <see cref="User"/>
            /// </summary>
            [Indexed]
            public int UserId { get; set; }

            /// <summary>
            /// String that represented the permission granted
            /// </summary>
            public string PermissionGranted { get; set; }
        }

        /// <summary>
        /// Song object
        /// </summary>
        public class Song
        {
            /// <summary>
            /// Automatically assigned identifer for this object
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int SongId { get; private set; }

            /// <summary>
            /// User id <see cref="User"/> that added this song
            /// </summary>
            [Indexed]
            public int UserId { get; set; }

            /// <summary>
            /// Spotify id of song
            /// </summary>
            public string SpotifyId { get; set; }

            /// <summary>
            /// Date the song was queued
            /// </summary>
            public DateTime DateAddedUtc { get; set; }
        }
    }
}
