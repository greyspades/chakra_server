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
            Hi {_data?.FirstName}
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
            <ul style='color: black; font-size: 14px; font-weight: normal;'>
                <li style='color: black; font-size: 14px; font-weight: normal;'>
                     Link: click <a href={_data?.Link}>here</a> to join
                </li>
                <li> Meeting ID: {_data?.MeetingId}</li>
                <li> Passcode: {_data?.Password}</li>
            </ul>
        </ul>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        We look forward to meeting you!
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        Kind regards,
    </div>
</body>
<footer style='color: black; font-size: 14px; font-weight: normal;'>
    <div style='color: black; font-size: 14px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row; color: black; font-size: 14px; font-weight: normal;'>
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
    <div style='color: black; font-size: 14px; font-weight: normal;'>
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
            Dear {candidate?.FirstName}
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
    <div>
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
            Dear {candidate?.FirstName}
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
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    The Talent Acquisition Team
    </p>
    <p style='color: black; font-size: 14px; font-weight: normal;'>
    LAPO Microfinance Bank
    </p>
</body>
<footer style='color: black; font-size: 14px; font-weight: normal; padding: 10px'>
    <div style='color: black; font-size: 14px; font-weight: normal;'><p>LAPO MfB Careers Team  </p></div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Head Office:</p>
        <p>LAPO Development Centre, 15 Ikorodu Road, Maryland Bus Stop, Lagos, Nigeria.</p>
    </div>
    <div style='flex:auto; flex-direction: row;'>
        <p style='color: orange; font-weight: 400;'>Annex:</p>
        <p>LAPO Place, 18 Dawson Road, P.M.B 1729, Benin City, Edo State. Nigeria.</p>
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
        +2348063803863 
    </div>
    <div style='color: black; font-size: 14px; font-weight: normal;'>
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
        <p>
            Hi {payload.FirstName}
        </p>
    </div>
    <div>
        <p>
            Please Confirm your email address by clicking <a href='/localhost:3000/emailConfirmation?email={payload.Email}'>here</a>
        </p>
    </div>
    <div>
        We look forward to meeting you!
    </div>
    <div>
        Kind regards,
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