using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using SPT.Helpers;

namespace SPT.Models
{
	public class User : Model
	{
		[BsonId]
		[BsonElement("_id")]
		public ObjectId _id { get; set; }
		public string FullName { get; set; }
		public DateTime Birthdate { get; set; }
		public string Login
		{
			get
			{
				return CreateLoginFromFullName(FullName);
			}
			set
			{
				
			}
		}
		public string PassHash { get; set; }
		public Roles Role { get; set; }

		public User()
		{
			_id = ObjectId.GenerateNewId();
		}
		 
		public bool haveActiveInstructionProgramm
		{
			get
			{
				var instrucionProgrammsCount = (new InstructionProgramm()).
					GetData<InstructionProgramm>().
					Where(i => i.UserId == _id).Count();

				return (instrucionProgrammsCount > 0) ? true : false;
			}
		}

		public string CreateLoginFromFullName(string fullName)
		{
			string login = String.Empty;

			string fullNAmeTranslitted = Translitter.Translit(fullName);
			string[] fullNameSplitted = fullNAmeTranslitted.Split(new char[] { ' ' });

			string surname = fullNameSplitted[0];
			string initials = String.Empty;

			for(int i = 1; i < fullNameSplitted.Length; i++)
			{
				initials += fullNameSplitted[i][0];
			}

			login = surname + initials;

			return login;
		}
	}
}
