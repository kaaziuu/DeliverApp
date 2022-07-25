import { Autocomplete, DateTimePicker, LoadingButton, LocalizationProvider } from "@mui/lab";
import { CircularProgress, TextField } from "@mui/material";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import TextFieldInput from "components/inputs/TextFieldInput";
import Snackbar from "components/snackbar/Snackbar";
import CreateDeliveryFrom from "service/deliveryService/models/CreateDeliveryForm";
import UpdateDeliveryFrom from "service/deliveryService/models/UpdateDeliveryForm";
import { GetLocationListAcion } from "service/location/LocationService";
import { PermisionToActionEnum } from "service/userService/models/Permissions";
import { HasPermission } from "service/userService/Roles";
import { LocationFullText } from "utils/location/LocationUtils";
import CreateClass from "utils/style/CreateClass";

interface props {
    baseClassName: string;
    form: CreateDeliveryFrom | UpdateDeliveryFrom;
    error: string | null;
    showSnackbar: boolean;
    title: string;
    submitButtonText: string;
    successMessage: string;
    isLoading: boolean;

    setShowSnackbar: (value: boolean) => void;
    setForm: (value: any) => void;
    submitForm: () => Promise<void>;
}

const DeliverForm = ({
    baseClassName,
    form,
    setForm,
    showSnackbar,
    setShowSnackbar,
    error,
    submitButtonText,
    successMessage,
    title,
    isLoading,
    submitForm,
}: props) => {
    const locations = GetLocationListAcion();

    if (locations.isLoading) {
        return <CircularProgress />;
    }

    if (!locations.isSuccess) {
        if (HasPermission({ permissionAction: PermisionToActionEnum.get, permissionTo: "location" })) {
            return <>please refresh page</>;
        } else {
            return null;
        }
    }

    return (
        <LocalizationProvider dateAdapter={AdapterDateFns}>
            <div className={baseClassName}>
                <h1 className={CreateClass(baseClassName, "header")}>{title}</h1>
                <div className={CreateClass(baseClassName, "fields")}>
                    <TextFieldInput
                        value={form.name}
                        baseClass={baseClassName}
                        label="Name"
                        onChange={(e) => setForm({ ...form, name: e.target.value })}
                        type="text"
                    />
                    <Autocomplete
                        disablePortal
                        value={locations.data!.find((x) => x.hash === form.fromLocationHash)}
                        options={locations.data!}
                        renderInput={(param) => <TextField {...param} label="From" />}
                        getOptionLabel={(location) => LocationFullText(location)}
                        onChange={(e, value) => {
                            setForm({ ...form, fromLocationHash: value!.hash });
                        }}
                    />
                    <Autocomplete
                        disablePortal
                        value={locations.data!.find((x) => x.hash === form.toLocationHash)}
                        options={locations.data!}
                        renderInput={(param) => <TextField {...param} label="To" />}
                        getOptionLabel={(location) => LocationFullText(location)}
                        onChange={(e, value) => {
                            setForm({ ...form, toLocationHash: value!.hash });
                        }}
                    />
                    <DateTimePicker
                        maxDate={form.endDate}
                        label="pickup time"
                        value={form.startDate}
                        onChange={(newDate: Date | null) => setForm({ ...form, startDate: newDate! })}
                        renderInput={(params: any) => <TextField {...params} />}
                    />
                    <DateTimePicker
                        minDate={form.startDate}
                        label="deliver time"
                        value={form.endDate}
                        onChange={(newDate: Date | null) => setForm({ ...form, endDate: newDate! })}
                        renderInput={(params: any) => <TextField {...params} />}
                    />
                </div>
                <LoadingButton
                    loading={isLoading}
                    className={CreateClass(baseClassName, "submit")}
                    size="large"
                    onClick={submitForm}
                    color="success"
                    variant="outlined"
                >
                    {submitButtonText}
                </LoadingButton>
            </div>
            <Snackbar
                message={error ? error : successMessage}
                setIsOpen={setShowSnackbar}
                variant={error ? "error" : "success"}
                isOpen={showSnackbar}
            />
        </LocalizationProvider>
    );
};

export default DeliverForm;
