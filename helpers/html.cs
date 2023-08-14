using Microsoft.AspNetCore.Html;
using Candidate.Models;

namespace HTML;

public static class HTMLHelper {
    // private readonly MeetingDto? _data;
    // private readonly CandidateModel _candidate;

    // public HTMLHelper(MeetingDto data) {
    //     _data = data;
    // }

    // public HTMLHelper(CandidateModel candidate)
    // {
    //     _candidate = candidate;
    // }

    public static string Interview(MeetingDto _data) {
        return $@"<html>
        <head>
    <meta charset='UTF-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Document</title>
</head>

<body style='padding: 10px; color: black;'>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        <p>
            Hi {_data?.FirstName},
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        <p>
            We reviewed your application for our {_data?.JobTitle} job and we are impressed with your background. We would like to have an interactive session with you as scheduled below.
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        <ul style='color: black; font-size: 14px; font-weight: normal;'>
            <li>Date: {_data?.Date}</li>
            <li>Time: {_data?.Time}</li>
            <li>Location: Zoom</li>
            <li style='color: black; font-size: 14px; font-weight: normal;'>
                Link: click <a href={_data?.Link}>here</a> to join
           </li>
           <li> Meeting ID: {_data?.MeetingId}</li>
           <li> Passcode: {_data?.Password}</li>
        </ul>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal; margin-top: 20px;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Head Office:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
    }

    public static string Rejection(FlagCandidateDto role, CandidateModel candidate) {
        return $@"<html>
<body style='color: black; font-size: 14px; font-weight: normal; padding: 10px'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Dear {candidate?.FirstName},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Thank you for giving us the opportunity to consider you for employment. We have reviewed your profile but regret that at this time we will not be moving forward with your application as a {role.RoleName}. 
        </p>
    </div>

    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            With your approval, we will be keeping your resume for future job openings when they arise.
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
        We wish you every success with your job search and thank you for your interest in our bank. 
        </p>
    </div>
    <div>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    Sincerely,
    </p>
    </div>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    The Talent Acquisition Team
    </p>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    LAPO Microfinance Bank
    </p>
</body>
<footer style='color: black; font-size: 14px; font-weight: normal; padding: 10px;'>
    <div style='color: black; font-size: 14px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p style='color: black; font-size: 14px; font-weight: normal;'>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Annex:</p>
        <p style='color: black; font-size: 14px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
    }

    public static string Acknowledgement(CandidateModel candidate) {
        return $@"<html>
<body style='color: black; font-size: 14px; font-weight: normal; padding: 10px;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Dear {candidate?.FirstName},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Thanks for your interest in LAPO Microfinance Bank. We have received your application for our {candidate?.JobName} role and we will review it right away.
        </p>
    </div>

    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            What happens now? We will review your application within 5 days and will contact you if there is a good match. However, due to the vast quantity of applications received, we may not be able to contact you if you are not shortlisted. 
        </p>
    </div>
    <div>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    Sincerely,
    </p>
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal; padding: 10px'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Head Office:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
    }

public static string PasswordReset(PasswordResetFields payload) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Dear {payload.FirstName}
        </p>
        <div style='color: black; font-size: 14px; font-weight: normal;'>
            To reset your password please click <a href='http://192.168.1.28:8089/password_reset?email={payload.Email}&token=00727143910'>here</a>
        </div>
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}

public static string OfferEmail(OfferMailDto payload) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>

    <div style='text-decoration: underline; font-weight: 600;'>
        Provisional Offer of Employment 
    </div>
    <div>
        <p>
            Dear {payload.FirstName},
        </p>
    </div>
    <div>
        Welcome to LAPO Microfinance Bank! We are pleased to offer you provisional employment to bring you on board our mission to improve lives.
    </div>
    <div style='margin-top: 20px;'>
        Your full-time employment as <span>{payload.Position}</span> (if you accept our offer) will be according to the terms set out in the contracts of employment and other documentation and as stated in the attached document.
    </div>
    <div style='margin-top: 20px;'>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Congratulations and looking forward to hearing from you.
        </p>
    </div>

</body>
<footer style='padding: 20px;'>
    <div><p style='color: black; font-size: 11px; font-weight: normal;'>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Head Office:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}
public static string Acceptance(CandidateModel payload) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>

    <div style='text-decoration: underline; color: black; font-size: 11px; font-weight: normal;'>
        Provisional Offer of Employment 
    </div>
    <div>
        <p style='color: black; font-size: 11px; font-weight: normal;'>
            Dear {payload.FirstName}
        </p>
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        Welcome to LAPO Microfinance Bank! We are pleased to offer you provisional employment to bring you on board our mission to improve lives.
    </div>
    <div style='margin-top: 20px; color: black; font-size: 11px; font-weight: normal;'>
        Your full-time employment as <span>{payload.JobName}</span> (if you accept our offer) will be according to the terms set out in the contracts of employment and other documentation and as stated in the attached document.
    </div>
    <div style='margin-top: 20px;'>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Congratulations and looking forward to hearing from you.
        </p>
    </div>
</body>
<footer style='padding: 20px;'>
    <div><p style='color: black; font-size: 11px; font-weight: normal;'>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Head Office:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400; font-size: 11px;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}
public static string VerifyEmail(BasicInfo payload) {
    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Hi {payload.FirstName},
        </p>
    </div>
    <div>
        <p style='color: black; font-size: 14px; font-weight: normal;'>
            Please Confirm your email address by clicking <a href='http://localhost:8089/emailConfirmation?email={payload.Email}'>here</a>
        </p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal; margin-top: 20px;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 11px; font-weight: normal;'>
    <div style='color: black; font-size: 11px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 11px; font-weight: normal;'>
        <p style='color: orange; color: black; font-size: 11px; font-weight: normal;'>Annex:</p>
        <p style='color: black; font-size: 11px; font-weight: normal;'>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 11px; font-weight: normal;'>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}
public static string OfferLetter(HireDto payload) {

    return $@"<!DOCTYPE html>
<html>
<body style='padding: 10px; color: black; font-size: 14px; font-weight: normal;'>
    <div>
        <p>{payload.Date}</p>
    </div>
    <div>
        <p>{payload.FirstName + ' ' + payload.LastName}</p>
        <p>{payload.Address}</p>
        <p>{payload.City}</p>
    </div>

    <div style='text-decoration: underline; font-weight: 600;'>
        Provisional Offer of Employment 
    </div>
    <div>
        <p>
            Dear {payload.FirstName}
        </p>
    </div>
    <div>
        Welcome to LAPO Microfinance Bank! We are pleased to offer you provisional employment to bring you on board our mission to improve lives.
    </div>
    <div style='margin-top: 20px;'>
        Your {payload.JobType} employment as <span>{payload.Position}</span> (if you accept our offer) will be according to the terms set out in the contracts of employment and other documentation and as stated in the attached document.
    </div>
    <div>
        <p style='font-weight: bold;'>1.	Job Description and commencement:</p>
        <ul>
            <li>Your position: 	-{payload.Position} </li>
            <li>Rank:	-{payload.Rank}</li>
            <li>Reports to:	 -{payload.ReportTo} </li>
            <li>Workplace location:  -{payload.Location}</li>
            <li>Expected start date: -{payload.StartDate}. You are required to complete the documentation procedure which forms part of this offer.</li>
        </ul>
    </div>
    <div>
        <p style='font-weight: bold;'>2. Induction</p>
        On your start date and upon satisfactory completion of the documentation procedure, you would be formally on-boarded via induction/training programme titled: ‘Comprehensive Training in Basic Microfinance.’ The length of the training programme shall be communicated upon resumption.
    </div>
    <div>
        <p style='font-weight: bold;'>3. Remuneration and benefits</p>
        <ul>
            <li><p>Salary</p style='font-weight: bold;'>o	The current annual gross salary for this position is N{payload.Salary} (One million, two hundred-thousand-naira, zero kobo only).</li>
            <li><p style='font-weight: bold;'>Deductions</p>o	Statutory deductions such as Contributory Pension, tax, NHF, etc. would be made monthly on your gross salary stated above.</li>
            <li><p style='font-weight: bold;'>Other benefits</p><p>o	13th Month: Subject to company policy</p>
            <p>o	Leave Transport Grant: 7% of annual basic salary per annum</p>
            <p>o	Group Life Insurance:  Covered</p>
            <p>o	Employees Compensation:  Covered</p>
            <p>o	Health (Medical) Insurance: Covered</p>
            </li>

            <li><p style='font-weight: bold;'>Vacation/leave:</p><p>o	Annual Leave: 	Ten (10) working days per annum</p>
                <p>o	Casual Leave: 	Seven (7) working days per annum</p>
                <p>o	Maternity Leave: Three (3) months </p>
                <p>o	Sick Leave: Twenty (20) calendar days per annum and upon certification by a licensed physician.</p>
                <p>o	Compassionate Leave: Ten (10) working days per case</p>
                <p>o	Study Leave: Subject to company policy</p>
            </li>

            <li><p style='font-weight: bold;'>•	Public holiday:</p><p>o	Holidays approved by the Federal Government of Nigeria</p>

                </li>
        </ul>
    </div>
    <div>
        <p style='font-weight: bold;'>4.	Probation:</p>
        <p>
            You will undergo a probationary period of six (6) months from your employment commencement date. During the probation period, your continued employment will be determined. Subject to satisfactory performance and fulfilment of all the conditions of employment; your employment may be confirmed. During the probation period, your employment may be terminated on the grounds of persistent misconduct, poor performance and sundry breaches.
        </p>
    </div>

    <div>
        <p style='font-weight: bold;'>5.	Key Performance Indicator (KPI) / Target:</p>
        <p>
            A document containing your targets is attached to this offer and your productivity will be measured using the Key Performance Indicators (KPIs) for your position.
        </p>
    </div>

    <div>
        <p style='font-weight: bold;'>6.	Hours of employment:</p>
        <p>
            The normal working hours of employment shall be 8:00 am to 5:00 pm Monday through Friday during which time you may take up to one hour for lunch between the hours of 12:30 and 3:00 pm. You may from time to time be required to work such additional hours as is reasonable to meet the requirements of the employer’s business.
        </p>
    </div>
    <div>
        <p style='font-weight: bold;'>7.	Collective agreements:</p>
        <p>
            There are no collective agreements in force directly relating to the terms of your employment.
        </p>
    </div>

    <div>
        <p style='font-weight: bold;'>8.	Pension:</p>
        <p>
            You shall be entitled to select a licensed Pension Fund Administrator (PFA) of your choice, the details of which are set out in the bank’s Employee Handbook which is available on request.
        </p>
    </div>

    <div>
        <p style='font-weight: bold;'>9.	Termination:</p>
        <p>
            Either party may terminate this employment by giving written notice or payment in lieu of such notice as indicated below:
<p>a.	Management Staff: 	2 months 
    b.	Senior Staff: 		1 month 
    c.	Junior staff: 		1 month</p>
<p>
    Acceptance of notice of your resignation is contingent upon the following: 
a.	You are not under any form of investigation pending or outstanding concerning fraud, or any other serious misconduct. 
b.	You are not resigning to escape such an investigation
c.	You are not on suspension with respect to any case not yet fully investigated 
d.	You have not committed (or been found guilty of) any offence that would earn a termination of appointment, summary dismissal or require the matter to be reported to the police.
The bank shall pay to you, all due entitlements on cessation of employment. Similarly, all indebtedness to the company shall become payable on cessation of this employment and all the bank’s property in your possession must be returned immediately.
</p>

        </p>
    </div>

    <div>
        <p style='font-weight: bold;'>10.	Confidentiality and Non-disclosure:</p>
        <p>
            Please be aware that during your employment you may be party to confidential information concerning the employer and the employer’s business. You shall not during the term of your employment, or three years after, disclose or allow the disclosure of any confidential information (except in the proper course of your employment). 
        </p>
        <p>The employer shall keep confidential of all data obtained concerning you and agrees not to use the data for any other purpose except for that which it was obtained and in compliance with the Nigeria Data Protection Regulation or laws as would be made periodically.</p>
        <p>After the termination of your employment, you shall not disclose or use any of the employer’s trade secrets or any other information which is of a sufficiently high degree of confidentiality to amount to a trade secret, for a period not less than three years. The employer shall be entitled to apply for an injunction to prevent such disclosure or use and to seek any other remedy including without limitation the recovery of damages in the case of such disclosure or use.</p>
        <p>The obligation of confidentiality both during and after the termination of your employment shall not apply to any information which you are enabled to disclose under the applicable Federal Laws provided you have first fully complied with the employer’s procedures relating to such external disclosures.</p>
    </div>
    <div>
        <p style='font-weight: bold;'>11.	Non-Competition:</p>
        <p>
            For a period of three years after the termination of your employment, you shall not solicit or seek business from any customers or clients of the employer who were customers or clients of the employer at the time during the 12 months immediately preceding the termination of your employment with whom you had material dealings.
        </p>
        <p>While your employment with the employer subsists, you are required to disclose to the employer any potential conflicts of interest including concurrent employment outside the company which may impact negatively on either the Bank’s performance or personal performance.</p>
    </div>
    <div>
        <p style='font-weight: bold;'>12.	Discipline and grievance:</p>
        <p>
            The employer’s disciplinary rules, procedure, grievance and appeal procedure in connection with these rules are set out in the Employee Handbook.
        </p>
    </div>
    <div>
        <p style='font-weight: bold;'>13.	Notices:</p>
        <p>
            All communications including notices required to be given under this offer shall be in writing and emails and/or shall be sent either electronically, by personal service or by first-class post to the parties’ respective current addresses.
        </p>
    </div>
    <div>
        <p style='font-weight: bold;'>14.	Severability:</p>
        <p>
            If any provision of this offer should be held to be invalid it shall to that extent be severed and the remaining provisions shall continue to have full force and effect.
        </p>
    </div>
    <div>
        <p style='font-weight: bold;'>15.	Employee handbook:</p>
        <p>
            Further details of the arrangements affecting your employment are published in the Employee Handbook as issued and/or amended from time to time. These are largely of an administrative nature but, so far as relevant, are to be treated as incorporated in this offer.
        </p>
    </div>
    <div>
        <p style='font-weight: bold;'>16.	Background Screening:</p>
        <p>
            This offer is contingent on a successful background screening (this includes verification of your identity, result and guarantors. Others include a criminal record check and depending on your role, a credit check); your eligibility to work in Nigeria and of course, your acceptance.
        </p>
        <p>If you are willing and ready to join us, please acknowledge receipt of this offer and your agreement to the terms set out in it by signing below and sending the signed copy back to us within forty-eight hours. </p>
        <p>Congratulations and looking forward to hearing from you.</p>
    </div>
    <div>
        <p>Oludare Obadina</p>
        <p style='font-weight: bold;'>Head, Human Resources</p>
    </div>

    <div>
        <div>
            <p style='margin-top: 10px;'>Agreed to and accepted by: </p>
            <hr />
        </div>
        <div>
            <p style='margin-top: 10px;'>Sign:</p>
            <hr />
        </div>
        <div>
            <p style='margin-top: 10px;'>Date:</p>
            <hr />
        </div>
    </div>

</body>
<footer style='padding: 20px;'>
    <div><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Annex:</p>
        <p>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div>
        +2348063803863 
    </div>
    <div>
        paul.obasuyi@lapo-nigeria.org 
        www.lapo-nigeria.org   
    </div>
</footer>
</html>";
}
}