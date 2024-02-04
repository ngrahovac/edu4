import React, { useState } from 'react'
import LandingFlair from '../comps/landing/LandingFlair'

const PainPoints = () => {
    const hatTypes = [
        { id: 1, value: "student" },
        { id: 2, value: "academic/researcher" },
        { id: 3, value: "industry professional" }
    ];

    const painPoints = {
        "academic/researcher": [],
        "student": [{
            problem: "You’ve been thinking about an idea for months, but have nobody to help bring it to life?",
            solution: "Describe the collaborators you’re looking for and reach just the right people!"
        },
        {
            problem: "Your team is working on a SaaS solution, but not one of you can design to save their life?",
            solution: "Find designers and frontend developers to help bring everything to another level."
        },
        {
            problem: "Want to enrich your studies by putting your newly acquired knowledge into practice?",
            solution: "Discover opportunities for working on projects with your peers or the teaching staff."
        },
        {
            problem: "Your team is cooking up something good, but could really use the help of a seasoned professional?",
            solution: "Specify the kind of mentor you’re looking for and reach an industry veteran."
        }],
        "industry professional": []
    }

    const [selectedHatType, setSelectedHatType] = useState("student");

    return (
        <div className='text-lg text-gray-600 flex flex-col gap-y-4'>
            <div className='flex gap-x-4'>
                <p className='py-1'>You're a</p>
                <div className='flex gap-x-2'>
                    {
                        hatTypes.map(ht => <div key={ht.id} onClick={() => { setSelectedHatType(ht.value) }}>
                            <LandingFlair
                                selected={selectedHatType == ht.value}>
                                {ht.value}
                            </LandingFlair>
                        </div>)
                    }
                </div>
            </div>
            {
                painPoints[selectedHatType].slice(0, 1).map(pp => <div className='flex flex-col gap-y-1'>
                    <p className='text-gray-700 font-semibold'>{pp.problem}</p>
                    <p className='text-gray-700'>{pp.solution}</p>
                </div>)
            }
        </div>
    )
}

export default PainPoints