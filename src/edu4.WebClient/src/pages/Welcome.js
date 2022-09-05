import React from 'react';

const Welcome = () => {
    return (
        <>
            <div className='flex flex-row absolute top-2 right-4 z-10'>
                <button
                    type="button"
                    className='px-4 py-2 rounded-md bg-blue-500 hover:bg-blue-600 text-stone-50 font-semibold text-lg'
                    onClick={() => window.alert("hi")}>
                    Sign up
                </button>

                <button
                    type="button"
                    className='px-4 py-2 rounded-md bg-stone-200 hover:bg-stone-300 text-slate-800 font-semibold text-lg ml-2'>
                    Login
                </button>
            </div>

            <div
                className='w-full h-full text-center pt-64 text-slate-700 absolute bottom-0 align-middle text-3xl'>
                <p className='font-bold text-5xl'>
                    edu4
                </p>
                <p className='mt-4'>
                    helps you find collaborators for bringing your ideas to life.
                </p>
            </div>
        </>
    )
}

export default Welcome
