using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Results
    {
        private string gottenPic;
        private string nik;
        private string name;
        private string birthDate;
        private string birthLoc;
        private string gender;
        private string bloodType;
        private string address;
        private string religion;
        private string marriageStatus;
        private string job;
        private string citizenship;
        private double execTime;
        private double matchPercentage;

        public Results()
        {
            gottenPic = string.Empty;
            nik = string.Empty;
            name = string.Empty;
            birthDate = string.Empty;
            birthLoc = string.Empty;
            gender = string.Empty;
            bloodType = string.Empty;
            address = string.Empty;
            religion = string.Empty;
            marriageStatus = string.Empty;
            job = string.Empty;
            citizenship = string.Empty;
            execTime = 0;
            matchPercentage = 0;
        }

        public string getGottenPic()
        {
            return gottenPic;
        }
        public string getNik()
        {
            return nik;
        }
        public string getName()
        {
            return name;
        }
        public string getBirthDate()
        {
            return birthDate;
        }
        public string getBirthLoc() { 
            return birthLoc; 
        }
        public string getGender()
        {
            return gender;
        }
        public string getAddress()
        {
            return address;
        }
        public string getReligion() // YOU WANT FORGIVENESS?
        {
            return religion;
        }
        public string getMarriageStatus()
        {
            return marriageStatus;
        }
        public string getJob()
        {
            return job;
        }
        public string getCitizenship()
        {
            return citizenship;
        }
        public double getExecTime()
        {
            return execTime;
        }
        public double getMatchPercentage()
        {
            return matchPercentage;
        }
        public void setGottenPic(string gottenPic)
        {
            this.gottenPic = gottenPic;
        }
        public void setNik(string nik)
        {
            this.nik = nik;
        }
        public void setName(string name)
        {
            this.name = name;
        }
        public void setBirthDate(string birthDate)
        {
            this.birthDate = birthDate;
        }
        public void setBirthLoc(string birthLoc)
        {
            this.birthLoc = birthLoc;
        }
        public void setGender(string gender)
        {
            this.gender = gender;
        }
        public void setBloodType(string bloodType)
        {
            this.bloodType = bloodType;
        }
        public void setAddress(string address)
        {
            this.address = address;
        }
        public void setReligion(string religion)
        {
            this.religion = religion;
        }
        public void setMarriageStatus(string marriageStatus)
        {
            this.marriageStatus = marriageStatus;
        }
        public void setJob(string job)
        {
            this.job = job;
        }
        public void setCitizenship(string citizenship)
        {
            this.citizenship = citizenship;
        }
        public void setExecTime(double execTime)
        {
            this.execTime = execTime;
        }
        public void setMatchPercentage(double matchPercentage)
        {
            this.matchPercentage = matchPercentage;
        }
        public void setAll(string gottenPic,
                           string nik,
                           string name,
                           string birthDate,
                           string birthLoc,
                           string gender,
                           string bloodType,
                           string address,
                           string religion,
                           string marriageStatus,
                           string job,
                           string citizenship,
                           double execTime,
                           double matchPercentage)
        {
            setAddress(address);
            setReligion(religion);
            setBirthDate(birthDate);
            setBloodType(bloodType);
            setCitizenship(citizenship);
            setGender(gender);
            setGottenPic(gottenPic);
            setJob(job);
            setMarriageStatus(marriageStatus);
            setName(name);
            setNik(nik);
            setBirthLoc(birthLoc);
            setExecTime(execTime);
            setMatchPercentage(matchPercentage);
        }
    }
}
