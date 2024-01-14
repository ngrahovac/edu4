import React from 'react'

const StudentHat = ({ hat }) => {
    let degree = hat.parameters.academicDegree == 1 ?
        "BSc" :
        hat.parameters.academicDegree == 2 ?
            "MSc" :
            "PHd";

    return (
        <div className='flex flex-wrap items-center font-semibold text-sm uppercase tracking-wide text-gray-400'>
            <div className='flex gap-x-2 shrink-0 items-center'>
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.042A8.967 8.967 0 0 0 6 3.75c-1.052 0-2.062.18-3 .512v14.25A8.987 8.987 0 0 1 6 18c2.305 0 4.408.867 6 2.292m0-14.25a8.966 8.966 0 0 1 6-2.292c1.052 0 2.062.18 3 .512v14.25A8.987 8.987 0 0 0 18 18a8.967 8.967 0 0 0-6 2.292m0-14.25v14.25" />
                </svg>

                <span>Student |</span>
            </div>
            <span>{'\u00A0'}</span>
            <span>{`${degree} |`}</span>
            <span>{'\u00A0'}</span>
            <span className='font-black'>{hat.parameters.studyField}</span>
        </div>
    )
}

export default StudentHat