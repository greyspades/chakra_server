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
}